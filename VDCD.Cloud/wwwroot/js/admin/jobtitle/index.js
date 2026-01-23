document.addEventListener("DOMContentLoaded", function () {
    const modalEl = document.getElementById("jobTitleModal");
    const form = document.getElementById("jobTitleForm");
    const tbody = document.getElementById("jobTitleTable");
    const searchInput = document.getElementById("searchInput");
    const pagination = document.getElementById("pagination");
    const jobTitleModal = new bootstrap.Modal(modalEl);

    let rows = Array.from(tbody.querySelectorAll("tr"));
    let filteredRows = [...rows];
    const rowsPerPage = 10;
    let currentPage = 1;

    // ===== SEARCH & PAGINATION =====
    function renderTable() {
        rows.forEach(r => r.style.display = "none");
        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;

        filteredRows.slice(start, end).forEach((r, idx) => {
            r.style.display = "";
            r.querySelector(".stt-cell").textContent = start + idx + 1;
        });
    }

    function renderPagination() {
        pagination.innerHTML = "";
        const totalPages = Math.ceil(filteredRows.length / rowsPerPage);
        if (totalPages <= 1) return;

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = `page-item ${i === currentPage ? "active" : ""}`;
            li.innerHTML = `<a class="page-link" href="javascript:void(0)">${i}</a>`;
            li.onclick = () => { currentPage = i; renderTable(); renderPagination(); };
            pagination.appendChild(li);
        }
    }

    searchInput.addEventListener("keyup", function () {
        const keyword = this.value.toLowerCase();
        filteredRows = rows.filter(r => r.dataset.name.toLowerCase().includes(keyword));
        currentPage = 1;
        renderTable();
        renderPagination();
    });

    // ===== CRUD OPERATIONS =====
    window.openCreateModal = function () {
        form.reset();
        document.getElementById("Id").value = "";
        jobTitleModal.show();
    };

    window.openEditModal = function (btn) {
        const row = btn.closest("tr");
        document.getElementById("Id").value = row.dataset.id;
        document.getElementById("JobTitleName").value = row.dataset.name;
        jobTitleModal.show();
    };

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const formData = new FormData(form);
        fetch("/Admin/JobTitle/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => {
                if (res.success) location.reload();
                else alert(res.message);
            });
    });

    window.deleteJobTitle = function (btn) {
        const row = btn.closest("tr");
        const id = row.dataset.id;
        if (confirm(`Xóa chức danh "${row.dataset.name}"?`)) {
            const formData = new FormData();
            formData.append("id", id);
            fetch("/Admin/JobTitle/Delete", { method: "POST", body: formData })
                .then(r => r.json())
                .then(res => {
                    if (res.success) location.reload();
                    else alert(res.message);
                });
        }
    };

    renderTable();
    renderPagination();
});