document.addEventListener("DOMContentLoaded", function () {
    // ===== BIẾN KHỞI TẠO =====
    const tbody = document.getElementById("positionTable");
    const rows = Array.from(tbody.querySelectorAll("tr"));
    const pagination = document.getElementById("pagination");

    const rowsPerPage = 10;
    let currentPage = 1;

    // ===== HÀM PHÂN TRANG =====
    function renderTable() {
        // Ẩn tất cả các dòng
        rows.forEach(r => r.style.display = "none");

        // Tính toán dòng hiển thị
        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;

        const visibleRows = rows.slice(start, end);
        visibleRows.forEach((r, index) => {
            r.style.display = "";
            // Cập nhật số thứ tự (STT) dựa trên trang hiện tại
            const sttCell = r.querySelector(".stt-cell");
            if (sttCell) sttCell.textContent = start + index + 1;
        });
    }

    function renderPagination() {
        pagination.innerHTML = "";
        const totalPages = Math.ceil(rows.length / rowsPerPage);

        if (totalPages <= 1) return; // Không hiện phân trang nếu chỉ có 1 trang

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = "page-item " + (i === currentPage ? "active" : "");
            li.innerHTML = `<a class="page-link" href="javascript:void(0)">${i}</a>`;
            li.onclick = (e) => {
                e.preventDefault();
                currentPage = i;
                renderTable();
                renderPagination();
            };
            pagination.appendChild(li);
        }
    }

    // Chạy lần đầu
    renderTable();
    renderPagination();

    // ===== CÁC HÀM MODAL (GIỮ NGUYÊN NHƯ PHẦN TRƯỚC) =====
    const modalEl = document.getElementById("positionModal");
    const form = document.getElementById("positionForm");
    const positionModal = new bootstrap.Modal(modalEl);

    window.openCreateModal = function () {
        form.reset();
        document.getElementById("Id").value = "";
        positionModal.show();
    };

    window.openEditModal = function (btn) {
        const row = btn.closest("tr");
        document.getElementById("Id").value = row.dataset.id;
        document.getElementById("PositionName").value = row.dataset.name || "";
        positionModal.show();
    };

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const formData = new FormData(form);
        fetch("/Admin/Position/Save", { method: "POST", body: formData })
            .then(r => r.json())
            .then(res => {
                if (res.success) location.reload();
                else alert(res.message);
            });
    });

    window.deletePosition = function (btn) {
        const row = btn.closest("tr");
        const id = row.dataset.id;
        if (confirm("Xác nhận xóa?")) {
            const formData = new FormData();
            formData.append("id", id);
            fetch("/Admin/Position/Delete", { method: "POST", body: formData })
                .then(r => r.json())
                .then(res => {
                    if (res.success) location.reload();
                });
        }
    };
});