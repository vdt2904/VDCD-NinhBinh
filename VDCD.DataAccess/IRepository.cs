using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VDCD.Entities.Custom;

namespace VDCD.DataAccess
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Gets(bool isReadOnly,
                            Expression<Func<T, bool>> spec = null,
                            Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                            params Func<IQueryable<T>, IQueryable<T>>[] postFilter);

        /// <summary>
        /// Tìm tất cả các phần tử phù hợp với điều kiện truyền vào. Kết quả này chỉ để đọc
        /// </summary>
        /// <param name="spec">Điều kiện tìm kiếm. Điều kiện có thể là Id > 1, Content != null... Khi điều kiện là null thì sẽ trả ra tất cả các phần tử</param>
        /// <param name="preFilter">Bộ lọc trước: Thay đổi, lọc dữ liệu trước khi truy vấn</param>
        /// <param name="postFilter">Bộ lọc sau: Thay đổi, lọc dữ liệu sau khi truy vấn được thực hiện. </param>
        /// <returns>Danh sách các thực thể</returns>
        IEnumerable<T> GetsReadOnly(Expression<Func<T, bool>> spec = null,
                            Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                            params Func<IQueryable<T>, IQueryable<T>>[] postFilter);

        /// <summary>
        /// Tìm tất cả các phần tử phù hợp với điều kiện truyền vào. Kết quả được ánh xạ sang một dạng khác bằng cách sử dụng một mapper do người dùng cung cấp. Kết quả này chỉ để đọc
        /// </summary>
        /// <typeparam name="TOutput">Kiểu đầu ra.</typeparam>
        /// <param name="projector">Là một công cụ để ánh xạ từ 1 kiểu thực thể sang 1 kiểu thực thể khác (nó tương đương Select column1, column2 From Table)</param>
        /// <param name="spec">Điều kiện tìm kiếm. Điều kiện có thể là Id > 1, Content != null... Khi điều kiện là null thì sẽ trả ra tất cả các phần tử</param>
        /// <param name="preFilter">Bộ lọc trước: Thay đổi, lọc dữ liệu trước khi truy vấn</param>
        /// <param name="postFilter">Bộ lọc sau: Thay đổi, lọc dữ liệu sau khi truy vấn được thực hiện. </param>
        /// <returns>Danh sách các thực thể được ánh xạ</returns>
        IEnumerable<TOutput> GetsAs<TOutput>(Expression<Func<T, TOutput>> projector,
                                                Expression<Func<T, bool>> spec = null,
                                                Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                                                params Func<IQueryable<T>, IQueryable<T>>[] postFilter);

        /// <summary>
        /// Tìm một phần tử bởi các thuộc tính nhận dạng (Id, Key)
        /// </summary>
        /// <param name="ids">Các giá trị của thuộc tính nhận dạng</param>
        /// <returns>Kiểu thực thể</returns>
        T Get(params object[] ids);

        /// <summary>
        /// Tìm một phần tử phù hợp với điều kiện kỹ thuật đã cho
        /// </summary>
        /// <param name="isReadOnly">True: Nếu kết quả trả ra chỉ để đọc</param>
        /// <param name="spec">Điều kiện</param>
        /// <returns>Kiểu thực thể</returns>
        T Get(bool isReadOnly, Expression<Func<T, bool>> spec);

        /// <summary>
        /// Tìm một phần tử bởi các thuộc tính nhận dạng (Id, Key). Kết quả chỉ để đọc
        /// </summary>
        /// <param name="spec">Điều kiện.</param>
        /// <returns>Kiểu thực thể</returns>
        T GetReadOnly(Expression<Func<T, bool>> spec);

        /// <summary>
        /// Tìm một phần tử phù hợp với điều kiện truyền vào, dữ liệu của phần tử đó sẽ được chuyển sang 1 kiểu khác. Kết quả chỉ để đọc
        /// </summary>
        /// <typeparam name="TOutput">Kiểu của đầu ra.</typeparam>
        /// <param name="projector">Công cụ ánh xạ.</param>
        /// <param name="specs">Điều kiện</param>
        /// <returns>Kiểu thực thể được ánh xạ</returns>
        TOutput GetAs<TOutput>(Expression<Func<T, TOutput>> projector,
                                Expression<Func<T, bool>> specs = null);

        /// <summary>
        /// Xác định xem có phần tử nào phù hợp với điều kiện truyền vào hay không
        /// </summary>
        /// <param name="spec">Điều kiện</param>
        /// <returns>Trả về true nếu tìm thấy và ngược lại</returns>
        bool Exist(Expression<Func<T, bool>> spec = null);

        /// <summary>
        /// Tính số lượng các phần tử phù hợp với được điểm kỹ thuật đã cho
        /// </summary>
        /// <param name="spec">Điều kiện kỹ thuật (điều kiện).</param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> spec = null);

        /// <summary>
        /// Tạo ra một phần tử mới trong db
        /// </summary>
        /// <param name="entity">Thực thể.</param>
        void Create(T entity);

        /// <summary>
        /// Xóa một phần tử trong db
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(T entity);
        void Update(T entity);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //int Count(Func<DashboardReport, bool> p);

        /// <summary>
        /// Lấy ra IQueryable.
        /// </summary>
        /// <value>The raw.</value>
        IQueryable<T> Raw { get; }
    }

}
