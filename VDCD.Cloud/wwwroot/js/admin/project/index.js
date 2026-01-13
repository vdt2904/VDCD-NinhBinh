document.addEventListener("DOMContentLoaded", function () {

    // ===== ELEMENTS =====
    const modalEl = document.getElementById("projectModal");
    const form = document.getElementById("projectForm");
    const contentList = document.getElementById("contentList");
    const tbody = document.getElementById("projectTable");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const pagination = document.getElementById("pagination");
    const searchInput = document.getElementById("searchInput");

    const projectModal = new bootstrap.Modal(modalEl);

    const rowsPerPage = 10;
    let currentPage = 1;
    let filteredRows = [...rows];

    // ===== SEARCH =====
    searchInput.addEventListener("keyup", function () {
        const keyword = this.value.toLowerCase();
        filteredRows = rows.filter(r =>
            (r.dataset.name || "").toLowerCase().includes(keyword)
        );
        currentPage = 1;
        renderTable();
        renderPagination();
    });

    // ===== PAGINATION =====
    function renderTable() {
        rows.forEach(r => r.style.display = "none");
        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        filteredRows.slice(start, end).forEach(r => r.style.display = "");
    }

    function renderPagination() {
        pagination.innerHTML = "";
        const totalPages = Math.ceil(filteredRows.length / rowsPerPage);
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = "page-item " + (i === currentPage ? "active" : "");
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.onclick = () => {
                currentPage = i;
                renderTable();
                renderPagination();
            };
            pagination.appendChild(li);
        }
    }

    renderTable();
    renderPagination();

    // ===== MODAL =====
    window.openCreateModal = function () {
        form.reset();  // vẫn reset các input khác
        document.getElementById("Id").value = "";   // xóa Id cũ
        document.getElementById("Json").value = ""; // xóa JSON cũ
        document.getElementById("ProjectImage").value = "";
        updateImagePreview("ProjectImage", "");     // xóa preview ảnh nếu có
        contentList.innerHTML = "";                 // xóa content blocks
        document.getElementById("IsActive").checked = false; // reset checkbox
        projectModal.show();
    };


    window.openEditModal = function (btn) {
        const row = btn.closest("tr");

        document.getElementById("Id").value = row.dataset.id;
        document.getElementById("ProjectName").value = row.dataset.name || "";
        document.getElementById("Investor").value = row.dataset.investor || "";
        document.getElementById("Time").value = row.dataset.time || "";
        document.getElementById("Location").value = row.dataset.location || "";
        document.getElementById("CategoryId").value = row.dataset.category || "";
        document.getElementById("IsActive").checked = row.dataset.isactive === "true";

        // Xử lý ảnh Project
        const imgPath = row.dataset.image || "";
        document.getElementById("ProjectImage").value = imgPath;
        updateImagePreview("ProjectImage", imgPath);

        loadContentList(row.dataset.json);
        projectModal.show();
    };

    // ===== IMAGE PREVIEW =====
    function updateImagePreview(inputId, url) {
        const previewDiv = document.getElementById('preview' + inputId);
        if (!previewDiv) return;
        if (url) {
            previewDiv.classList.remove('d-none');
            previewDiv.querySelector('img').src = url;
        } else {
            previewDiv.classList.add('d-none');
        }
    }

    // ===== CONTENT BLOCK =====
    function renderContentBlock(item = {}) {
        const blockId = 'img-' + Date.now() + Math.floor(Math.random() * 1000);
        return `
        <div class="content-block border rounded p-3 mb-3">
            <div class="d-flex justify-content-between mb-2">
                <strong>Nội dung</strong>
                <button type="button" class="btn btn-sm btn-danger"
                        onclick="this.closest('.content-block').remove()">Xoá</button>
            </div>
            <div class="mb-2">
                <label>Title</label>
                <input class="form-control content-title" value="${item.title || ""}">
            </div>
            <div class="mb-2">
                <label>Image</label>
                <div class="input-group">
                    <input class="form-control content-image" id="${blockId}" value="${item.image || ""}">
                    <button class="btn btn-outline-secondary" type="button" onclick="openFileManagerForBlock('${blockId}')">
                        <i class="fas fa-image"></i> Chọn ảnh
                    </button>
                </div>
            </div>
            <div class="mb-2">
                <label>Content</label>
                <textarea class="form-control content-content" rows="3">${item.content || ""}</textarea>
            </div>
        </div>`;
    }

    window.addContentBlock = function (item) {
        contentList.insertAdjacentHTML("beforeend", renderContentBlock(item));
    };

    function loadContentList(json) {
        contentList.innerHTML = "";
        if (!json) return;
        try {
            JSON.parse(json).forEach(x => addContentBlock(x));
        } catch (e) {
            console.error("JSON lỗi", e);
        }
    }

    function buildJson() {
        const list = [];
        contentList.querySelectorAll(".content-block").forEach(b => {
            list.push({
                title: b.querySelector(".content-title").value,
                image: b.querySelector(".content-image").value,
                content: b.querySelector(".content-content").value
            });
        });
        document.getElementById("Json").value = JSON.stringify(list);
    }

    // ===== SUBMIT =====
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        buildJson();

        const formData = new FormData(form);
        formData.set("IsActive", document.getElementById("IsActive").checked ? "true" : "false");

        fetch("/Admin/Projects/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => {
                if (res.success) location.reload();
                else alert(res.message || "Có lỗi xảy ra");
            })
            .catch(err => { console.error(err); alert("Lỗi server"); });
    });

    // ===== FILE MANAGER =====
    let currentActiveImgInputId = null;

    window.openFileManagerForBlock = function (inputId) {
        currentActiveImgInputId = inputId;
        const iframe = document.getElementById('fileManagerIframe');
        if (iframe) {
            iframe.src = '/Admin/FileManager/Index';
            const fmModal = new bootstrap.Modal(document.getElementById('fileManagerModal'));
            fmModal.show();
        }
    };
    // ===== DELETE PROJECT =====
    window.deleteProject = function (btn) {
        const row = btn.closest("tr");
        const id = row.dataset.id;
        const projectName = row.dataset.name;

        if (confirm(`Bạn có chắc chắn muốn xóa project "${projectName}" không?`)) {
            // Tạo FormData để gửi Id lên server (hoặc gửi qua URL tùy thuộc vào Action của bạn)
            const formData = new FormData();
            formData.append("id", id);

            fetch("/Admin/Projects/Delete", {
                method: "POST",
                body: formData
            })
                .then(r => r.json())
                .then(res => {
                    if (res.success) {
                        alert("Xóa thành công!");
                        location.reload(); // Tải lại trang để cập nhật danh sách
                    } else {
                        alert(res.message || "Có lỗi xảy ra khi xóa");
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Lỗi server khi thực hiện xóa");
                });
        }
    };
    window.addEventListener('message', function (event) {
        if (event.data && event.data.type === 'getFileUrl') {
            const fileUrl = event.data.url;
            if (currentActiveImgInputId) {
                const input = document.getElementById(currentActiveImgInputId);
                if (input) input.value = fileUrl;
                updateImagePreview(currentActiveImgInputId, fileUrl);
                currentActiveImgInputId = null;
            }
            const fmModalEl = document.getElementById('fileManagerModal');
            const fmModal = bootstrap.Modal.getInstance(fmModalEl);
            if (fmModal) fmModal.hide();
        }
    });

});
