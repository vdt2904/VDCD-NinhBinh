document.addEventListener("DOMContentLoaded", function () {
    const modalEl = document.getElementById("customerModal");
    const form = document.getElementById("customerForm");
    const tbody = document.getElementById("customerTable");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const pagination = document.getElementById("pagination");
    const searchInput = document.getElementById("searchInput");
    const customerModal = new bootstrap.Modal(modalEl);

    const rowsPerPage = 10;
    let currentPage = 1;
    let filteredRows = [...rows];

    // ===== TÌM KIẾM =====
    searchInput.addEventListener("keyup", function () {
        const kw = this.value.toLowerCase();
        filteredRows = rows.filter(r =>
            r.dataset.name.toLowerCase().includes(kw) ||
            (r.dataset.email || "").toLowerCase().includes(kw) ||
            (r.dataset.phone || "").toLowerCase().includes(kw)
        );
        currentPage = 1;
        renderTable();
        renderPagination();
    });

    // ===== PHÂN TRANG =====
    function renderTable() {
        rows.forEach(r => r.style.display = "none");
        const start = (currentPage - 1) * rowsPerPage;
        filteredRows.slice(start, start + rowsPerPage).forEach(r => r.style.display = "");
    }

    function renderPagination() {
        pagination.innerHTML = "";
        const totalPages = Math.ceil(filteredRows.length / rowsPerPage);
     //   if (totalPages <= 1) return;
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = `page-item ${i === currentPage ? "active" : ""}`;
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.onclick = (e) => { e.preventDefault(); currentPage = i; renderTable(); renderPagination(); };
            pagination.appendChild(li);
        }
    }

    renderTable(); renderPagination();

    // ===== ACTIONS =====
    window.openCustomerModal = function () {
        form.reset();
        document.getElementById("CustomerId").value = "0";
        document.getElementById("previewImage").src = "/images/no-avatar.png";
        customerModal.show();
    };

    window.editCustomer = function (btn) {
        const row = btn.closest("tr");
        document.getElementById("CustomerId").value = row.dataset.id;
        document.getElementById("CustomerName").value = row.dataset.name;
        document.getElementById("CustomerEmail").value = row.dataset.email;
        document.getElementById("CustomerPhone").value = row.dataset.phone;
        document.getElementById("CustomerAddress").value = row.dataset.address;
        document.getElementById("CustomerImage").value = row.dataset.image;
        document.getElementById("CustomerIsShow").checked = row.dataset.isshow === "true";
        document.getElementById("previewImage").src = row.dataset.image || "/images/no-avatar.png";
        customerModal.show();
    };

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const formData = new FormData(form);
        formData.set("IsShow", document.getElementById("CustomerIsShow").checked);

        fetch("/Admin/Customer/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => { if (res.success) location.reload(); else alert(res.message); });
    });

    window.deleteCustomer = function (btn) {
        if (confirm("Xóa khách hàng này?")) {
            const fd = new FormData();
            fd.append("id", btn.closest("tr").dataset.id);
            fetch("/Admin/Customer/Delete", { method: "POST", body: fd }).then(() => location.reload());
        }
    };

    // ===== FILE MANAGER =====
    let currentActiveImgInputId = null;
    window.openFileManagerForBlock = function (id) {
        currentActiveImgInputId = id;
        document.getElementById('fileManagerIframe').src = '/Admin/FileManager/Index';
        new bootstrap.Modal(document.getElementById('fileManagerModal')).show();
    };

    window.addEventListener('message', function (event) {
        if (event.data && event.data.type === 'getFileUrl') {
            const url = event.data.url;
            document.getElementById(currentActiveImgInputId).value = url;
            document.getElementById("previewImage").src = url;
            bootstrap.Modal.getInstance(document.getElementById('fileManagerModal')).hide();
        }
    });
});