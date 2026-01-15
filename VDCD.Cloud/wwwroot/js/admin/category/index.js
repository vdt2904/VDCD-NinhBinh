let categoryModal;


document.addEventListener("DOMContentLoaded", function () {
    categoryModal = new bootstrap.Modal(
        document.getElementById("categoryModal")
    );

    initFormSubmit();
    initSearchAndPaging();
});

/* ================= MODAL ================= */

function openCreateModal() {
    document.getElementById("modalTitle").innerText = "Thêm Category";
    document.getElementById("Id").value = 0;
    document.getElementById("CategoryName").value = "";
    document.getElementById("Description").value = "";

    categoryModal.show();
}

function openEditModal(btn) {
    const row = btn.closest("tr");

    document.getElementById("modalTitle").innerText = "Sửa Category";
    document.getElementById("Id").value = row.dataset.id;
    document.getElementById("CategoryName").value = row.dataset.name;
    document.getElementById("Description").value = row.dataset.desc;

    categoryModal.show();
}

/* ================= SAVE ================= */

function initFormSubmit() {
    const form = document.getElementById("categoryForm");
    if (!form) return;

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        if (!validateCategoryForm()) return;

        const formData = new FormData(form);
        for (const [key, value] of formData.entries()) {
            console.log(key, value);
        }
        fetch(categorySaveUrl, {
            method: "POST",
            body: formData
        })
            .then(r => r.json())
            .then(res => {
                if (res.success) {
                    location.reload();
                } else {
                    alert(res.message || "Có lỗi xảy ra");
                }
            })
            .catch(() => alert("Lỗi hệ thống"));
    });
    function validateCategoryForm() {
        let isValid = true;

        const nameInput = document.getElementById("CategoryName");
        const descInput = document.getElementById("Description");

        // reset trạng thái cũ
        [nameInput, descInput].forEach(el => {
            el.classList.remove("is-invalid");
        });

        // name required
        if (!nameInput.value.trim()) {
            nameInput.classList.add("is-invalid");
            isValid = false;
        }

        // description max length
        if (descInput.value.length > 500) {
            descInput.classList.add("is-invalid");
            isValid = false;
        }

        return isValid;
    }
}


/* ================= SEARCH + PAGING ================= */

function initSearchAndPaging() {
    const rowsPerPage = 10;
    let currentPage = 1;

    const table = document.getElementById("dataTable");
    const tbody = table.querySelector("tbody");
    const allRows = Array.from(tbody.querySelectorAll("tr"));

    const pagination = document.getElementById("pagination");
    const searchInput = document.getElementById("searchInput");

    function render() {
        const keyword = searchInput.value.toLowerCase();

        const filtered = allRows.filter(row =>
            row.innerText.toLowerCase().includes(keyword)
        );

        const totalPages = Math.max(1, Math.ceil(filtered.length / rowsPerPage));
        currentPage = Math.min(currentPage, totalPages);

        allRows.forEach(r => r.style.display = "none");

        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;

        filtered.slice(start, end).forEach(r => r.style.display = "");

        renderPagination(totalPages);
    }

    function renderPagination(totalPages) {
        pagination.innerHTML = "";

        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement("li");
            li.className = "page-item" + (i === currentPage ? " active" : "");

            const a = document.createElement("a");
            a.href = "#";
            a.className = "page-link";
            a.innerText = i;

            a.addEventListener("click", function (e) {
                e.preventDefault();
                currentPage = i;
                render();
            });

            li.appendChild(a);
            pagination.appendChild(li);
        }
    }

    searchInput.addEventListener("input", function () {
        currentPage = 1;
        render();
    });

    render();
}
