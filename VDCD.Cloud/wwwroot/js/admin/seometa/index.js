document.addEventListener("DOMContentLoaded", function () {
    const modalEl = document.getElementById("seoModal");
    const form = document.getElementById("seoForm");
    const tbody = document.getElementById("seoTable");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const pagination = document.getElementById("pagination");
    const searchInput = document.getElementById("searchInput");

    const seoModal = new bootstrap.Modal(modalEl);
    const rowsPerPage = 10;
    let currentPage = 1;
    let filteredRows = [...rows];

    // ===== SEARCH (Lọc theo Key hoặc Title) =====
    searchInput.addEventListener("keyup", function () {
        const kw = this.value.toLowerCase();
        filteredRows = rows.filter(r =>
            (r.dataset.key || "").toLowerCase().includes(kw) ||
            (r.dataset.title || "").toLowerCase().includes(kw)
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
        if (totalPages <= 1) return;

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = `page-item ${i === currentPage ? "active" : ""}`;
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
    window.openSeoModal = function () {
        form.reset();
        document.getElementById("SeoId").value = "0";
        document.getElementById("SeoIndex").checked = true;
        seoModal.show();
    };

    window.editSeo = function (btn) {
        const row = btn.closest("tr");
        document.getElementById("SeoId").value = row.dataset.id;
        document.getElementById("SeoKey").value = row.dataset.key || "";
        document.getElementById("SeoTitle").value = row.dataset.title || "";
        document.getElementById("SeoDesc").value = row.dataset.desc || "";
        document.getElementById("SeoKeywords").value = row.dataset.keywords || "";
        document.getElementById("SeoIndex").checked = row.dataset.index === "true";
        seoModal.show();
    };

    // ===== SUBMIT =====
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const formData = new FormData(form);
        // Xử lý checkbox Is_Index
        formData.set("Is_Index", document.getElementById("SeoIndex").checked ? "true" : "false");

        fetch("/Admin/Seo/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => {
                if (res.success) location.reload();
                else alert(res.message);
            })
            .catch(err => console.error(err));
    });

    // ===== DELETE =====
    window.deleteSeo = function (btn) {
        const row = btn.closest("tr");
        if (confirm(`Xóa cấu hình SEO cho: ${row.dataset.key}?`)) {
            const fd = new FormData();
            fd.append("id", row.dataset.id);
            fetch("/Admin/Seo/Delete", { method: "POST", body: fd })
                .then(r => r.json())
                .then(res => { if (res.success) location.reload(); });
        }
    };
});