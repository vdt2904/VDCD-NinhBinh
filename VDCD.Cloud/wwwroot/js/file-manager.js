var currentFolderId = null;

// Load folder tree
function loadFolders() {
    $.get("/api/FileManagerApi/folders", function (data) {
        function renderTree(nodes) {
            var html = "<ul>";
            nodes.forEach(function (node) {
                html += "<li data-id='" + node.id + "' class='folder-node'>" + node.name + "</li>";
                if (node.children && node.children.length > 0) {
                    html += renderTree(node.children);
                }
            });
            html += "</ul>";
            return html;
        }
        $("#folder-tree").html(renderTree(data));
    });
}

// Load files in folder
function loadFiles(folderId) {
    currentFolderId = folderId;
    $.get("/api/FileManagerApi/files", { parentId: folderId }, function (data) {
        var html = "";
        data.forEach(function (f) {
            if (f.isFolder) {
                html += `<div class='col'>
                            <div class='card p-2 folder-card' data-id='${f.id}'>
                                <i class='bi bi-folder'></i>
                                <div class='text-center'>${f.name}</div>
                            </div>
                         </div>`;
            } else {
                html += `<div class='col'>
                            <div class='card p-2 file-card' data-id='${f.id}'>
                                <img src='${f.url}' class='img-thumbnail'>
                                <div class='text-center small'>${f.name}</div>
                            </div>
                         </div>`;
            }
        });
        $("#file-grid").html(html);
    });
}

$(document).ready(function () {
    loadFolders();

    // Click folder node in tree
    $(document).on("click", ".folder-node", function () {
        var id = $(this).data("id");
        loadFiles(id);
    });

    // Double click folder in grid
    $(document).on("dblclick", ".folder-card", function () {
        var id = $(this).data("id");
        loadFiles(id);
    });

    // Upload button
    $("#upload-file").click(function () {
        $("#hidden-upload").click();
    });

    $("#hidden-upload").change(function () {
        var file = this.files[0];
        if (!file) return;
        var formData = new FormData();
        formData.append("file", file);
        formData.append("parentId", currentFolderId);

        $.ajax({
            url: "/api/FileManagerApi/upload",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                loadFolders();
                loadFiles(currentFolderId);
            }
        });
    });

    // Clear cache
    $("#clear-cache").click(function () {
        $.post("/api/FileManagerApi/clear-cache", function () {
            alert("Cache đã được xóa");
        });
    });
});
