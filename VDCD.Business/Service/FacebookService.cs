using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;
using static System.Net.WebRequestMethods;

namespace VDCD.Business.Service
{
    public class FacebookService
    {
        private readonly IRepository<FacebookPost> _fbRepo;
        private readonly IRepository<FacebookToken> _fbtRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRepository<SeoMeta> _seoRepo;
        private readonly HttpClient _http;
        public FacebookService(IRepository<FacebookPost> fbRepo,
            IRepository<FacebookToken> fbtRepo,
                              ICacheService cache,
                              AppDbContext context,
                              IRepository<SeoMeta> seoRepo,
                              HttpClient http)
        {
            _fbRepo = fbRepo;
            _cache = cache;
            _context = context;
            _seoRepo = seoRepo;
            _http = http;
            _fbtRepo = fbtRepo;
        }
        public void save(FacebookPost fbPost)
        {
            if(fbPost.Id == 0)
            {
                _fbRepo.Create(fbPost);
            }
            else
            {
                _fbRepo.Update(fbPost);
            }
            _context.SaveChanges();
            ClearCache();
        }
        /*        public async Task<FacebookToken> GetActiveTokenAsync(string pageId)
                {
                    var token = _fbtRepo.Get(false,x=>x.PageId ==pageId);

                    if (token == null)
                        throw new Exception("Không tìm thấy token fanpage");

                    // Nếu token sắp hết hạn trong 5 ngày
                    if (token.ExpiresAt.HasValue &&
                        token.ExpiresAt.Value < DateTime.UtcNow.AddDays(5))
                    {
                        await RefreshTokenAsync(token.Id);
                        token = _fbtRepo.Get(token.Id);
                    }

                    return token;
                }
                public async Task RefreshTokenAsync(int tokenId)
                {
                    var token = _fbtRepo.Get(tokenId);
                    if (token == null)
                        return;

                    // Đổi sang long-lived user token
                    var url =
                        $"https://graph.facebook.com/v19.0/oauth/access_token" +
                        $"?grant_type=fb_exchange_token" +
                        $"&client_id={_appId}" +
                        $"&client_secret={_appSecret}" +
                        $"&fb_exchange_token={token.UserAccessToken}";

                    var res = await _http.GetAsync(url);
                    var json = await res.Content.ReadAsStringAsync();

                    dynamic data = JsonConvert.DeserializeObject(json);

                    string newUserToken = data.access_token;
                    int expiresIn = data.expires_in;

                    token.UserAccessToken = newUserToken;
                    token.ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
                    token.LastRefreshDate = DateTime.UtcNow;

                    // Lấy lại page token
                    var pageUrl =
                        $"https://graph.facebook.com/v19.0/{token.PageId}" +
                        $"?fields=access_token" +
                        $"&access_token={newUserToken}";

                    var pageRes = await _http.GetAsync(pageUrl);
                    var pageJson = await pageRes.Content.ReadAsStringAsync();

                    dynamic pageData = JsonConvert.DeserializeObject(pageJson);
                    token.PageAccessToken = pageData.access_token;

                    _repo.Update(token);
                }*/
        public async Task<string> PostTextAsync(string pageId, string message, string pageToken)
        {
            var encodedMessage = Uri.EscapeDataString(message);

            var url = $"https://graph.facebook.com/v24.0/{pageId}/feed?message={message}&access_token={pageToken}";

            var response = await _http.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Facebook API Error: {json}");
            }

            return json;
        }

        public async Task<string> PostImagesAsync(string pageId,List<string> imageUrls,string message,string token)
        {
            var attachedMedia = new List<KeyValuePair<string, string>>();

            int index = 0;

            foreach (var img in imageUrls)
            {
                var uploadUrl = $"https://graph.facebook.com/v24.0/{pageId}/photos";

                var uploadContent = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("url", img),
            new KeyValuePair<string, string>("published", "false"),
            new KeyValuePair<string, string>("access_token", token)
        });

                var uploadResponse = await _http.PostAsync(uploadUrl, uploadContent);
                var uploadJson = await uploadResponse.Content.ReadAsStringAsync();

                if (!uploadResponse.IsSuccessStatusCode)
                    throw new Exception($"Upload image error: {uploadJson}");

                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadJson);
                string photoId = obj.id;

                attachedMedia.Add(
                    new KeyValuePair<string, string>(
                        $"attached_media[{index}]",
                        $"{{\"media_fbid\":\"{photoId}\"}}"
                    )
                );

                index++;
            }

            // Tạo post duy nhất
            var postUrl = $"https://graph.facebook.com/v24.0/{pageId}/feed";

            var postParams = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("message", message),
        new KeyValuePair<string, string>("access_token", token)
    };

            postParams.AddRange(attachedMedia);

            var postContent = new FormUrlEncodedContent(postParams);

            var postResponse = await _http.PostAsync(postUrl, postContent);
            var postJson = await postResponse.Content.ReadAsStringAsync();

            if (!postResponse.IsSuccessStatusCode)
                throw new Exception($"Create post error: {postJson}");

            return postJson;
        }

        public async Task<string> PostVideoAsync(string pageId,string videoUrl,string message,string token)
        {
            var url = $"https://graph.facebook.com/{pageId}/videos";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("file_url", videoUrl),
            new KeyValuePair<string, string>("description", message),
            new KeyValuePair<string, string>("access_token", token)
            });

            var res = await _http.PostAsync(url, content);
            return await res.Content.ReadAsStringAsync();
        }
        public async Task<string> PostVideoWithImagesAsync(string pageId,string videoUrl,List<string> imageUrls,string message,string token)
        {
            // 1️⃣ Đăng video trước
            var videoUrlEndpoint = $"https://graph.facebook.com/v24.0/{pageId}/videos";

            var videoContent = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("file_url", videoUrl),
        new KeyValuePair<string, string>("description", message),
        new KeyValuePair<string, string>("access_token", token)
    });

            var videoRes = await _http.PostAsync(videoUrlEndpoint, videoContent);
            var videoJson = await videoRes.Content.ReadAsStringAsync();

            if (!videoRes.IsSuccessStatusCode)
                throw new Exception($"Post video error: {videoJson}");

            dynamic videoObj = Newtonsoft.Json.JsonConvert.DeserializeObject(videoJson);
            string postId = videoObj.id;

            // 2️⃣ Upload ảnh unpublished
            var attachedMedia = new List<KeyValuePair<string, string>>();
            int index = 0;

            foreach (var img in imageUrls)
            {
                var uploadUrl = $"https://graph.facebook.com/v24.0/{pageId}/photos";

                var uploadContent = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("url", img),
            new KeyValuePair<string, string>("published", "false"),
            new KeyValuePair<string, string>("access_token", token)
        });

                var uploadRes = await _http.PostAsync(uploadUrl, uploadContent);
                var uploadJson = await uploadRes.Content.ReadAsStringAsync();

                if (!uploadRes.IsSuccessStatusCode)
                    throw new Exception($"Upload image error: {uploadJson}");

                dynamic imgObj = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadJson);
                string photoId = imgObj.id;

                attachedMedia.Add(
                    new KeyValuePair<string, string>(
                        $"attached_media[{index}]",
                        $"{{\"media_fbid\":\"{photoId}\"}}"
                    )
                );

                index++;
            }

            // 3️⃣ Comment album ảnh vào video post
            var commentUrl = $"https://graph.facebook.com/v24.0/{postId}/comments";

            var commentParams = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("access_token", token)
    };

            commentParams.AddRange(attachedMedia);

            var commentContent = new FormUrlEncodedContent(commentParams);

            var commentRes = await _http.PostAsync(commentUrl, commentContent);
            var commentJson = await commentRes.Content.ReadAsStringAsync();

            if (!commentRes.IsSuccessStatusCode)
                throw new Exception($"Comment images error: {commentJson}");

            return videoJson;
        }
        public IReadOnlyList<FacebookPost> GetAll()
        {
            if (_cache.TryGet(CacheParam.FacebookPostAll, out List<FacebookPost> cached))
                return cached;

            var data = _fbRepo
                .GetsReadOnly()
                .ToList();

            _cache.Set(
                CacheParam.FacebookPostAll,
                data,
                TimeSpan.FromMinutes(CacheParam.FacebookPostAllTimeout)
            );

            return data;
        }
        public FacebookPost Get(int id)
        {
            return _fbRepo.Get(id); 
        }
        private void ClearCache()
        {
            _cache.Remove(CacheParam.FacebookPostAll);
        }
    }
}
