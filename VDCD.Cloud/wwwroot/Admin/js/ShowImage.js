function validateImage(input, previewId, errorSpans) {
    var file = input.files[0];
    var preview = document.getElementById(previewId);
    var validExtensions = ["jpg", "jpeg", "png", "jfif"];
    var errorSpan = document.getElementById(errorSpans);
    if (file) {
        var extension = file.name.split('.').pop().toLowerCase();
        if (validExtensions.indexOf(extension) === -1) {
            errorSpan.textContent = "Chỉ nhận file có đuôi JPG, JPEG, PNG , JFIF !.";
            input.value = ''; // Xóa file đã chọn
            preview.src = ''; // Xóa hình ảnh xem trước
            return false;
        }
        errorSpan.textContent = "";
        var reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
        }
        reader.readAsDataURL(file);
    }
}
function displayImage(input,input2) {
    var preview = document.getElementById(input2);
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = '';
    }
}
