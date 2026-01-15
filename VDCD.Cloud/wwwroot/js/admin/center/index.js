document.addEventListener("DOMContentLoaded", function () {

    // ===== ELEMENTS =====
    const modalEl = document.getElementById("centerModal");
    const form = document.getElementById("centerForm");
    const tbody = document.getElementById("centerTable");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const pagination = document.getElementById("pagination");
    const searchInput = document.getElementById("searchInput");

    const centerModal = new bootstrap.Modal(modalEl);

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
      //  if (totalPages <= 1) return;

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = "page-item " + (i === currentPage ? "active" : "");
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.onclick = (e) => {
                e.preventDefault();
                currentPage = i;
                renderTable();
                renderPagination();
            };
            pagination.appendChild(li);
        }
    }

    renderTable();
    renderPagination();

    // ===== MODAL ACTIONS =====
    window.openCenterModal = function () {
        form.reset();
        document.getElementById("Id").value = "0";
        updateImagePreview("CenterImage", "");
        centerModal.show();
    };

    window.editCenter = function (btn) {
        const row = btn.closest("tr");
        document.getElementById("Id").value = row.dataset.id;
        document.getElementById("Name").value = row.dataset.name || "";
        document.getElementById("Description").value = row.dataset.description || "";
        document.getElementById("Url").value = row.dataset.url || "";
        document.getElementById("Icon").value = row.dataset.icon || "";

        const imgPath = row.dataset.image || "";
        document.getElementById("CenterImage").value = imgPath;
        updateImagePreview("CenterImage", imgPath);

        centerModal.show();
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

    // ===== SUBMIT FORM =====
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const formData = new FormData(form);
  //      const formData = new FormData(form);
        for (const [key, value] of formData.entries()) {
            console.log(key, value);
        }
        fetch("/Admin/Center/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => {
                if (res.success) location.reload();
                else alert(res.message || "Có lỗi xảy ra");
            })
            .catch(err => { console.error(err); alert("Lỗi server"); });
    });

    // ===== DELETE CENTER =====
    window.deleteCenter = function (btn) {
        const row = btn.closest("tr");
        const id = row.dataset.id;
        const name = row.dataset.name;

        if (confirm(`Bạn có chắc chắn muốn xóa center "${name}" không?`)) {
            const formData = new FormData();
            formData.append("id", id);

            fetch("/Admin/Centers/Delete", { method: "POST", body: formData })
                .then(r => r.json())
                .then(res => {
                    if (res.success) location.reload();
                    else alert(res.message || "Lỗi khi xóa");
                });
        }
    };

    // ===== FILE MANAGER (Iframe Message) =====
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