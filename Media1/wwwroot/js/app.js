document.addEventListener("DOMContentLoaded", () => {
    const catalogueView = document.getElementById("catalogueView");
    const uploadView = document.getElementById("uploadView");
    const navLinks = document.querySelectorAll("#viewTabs .nav-link");
    const videoTable = document.getElementById("videoTable");
    const videoPlayer = document.getElementById("videoPlayer");
    const videoPlayerContainer = document.getElementById("videoPlayerContainer");
    const catalogueMessage = document.getElementById("catalogueMessage");
    const uploadForm = document.getElementById("uploadForm");
    const uploadAlert = document.getElementById("uploadAlert");

    // Toast helper
    function showUploadToast(message, type = "success") {
        const toastBody = document.getElementById("uploadToastBody");
        const toastEl = document.getElementById("uploadToast");

        // Update text and color
        toastBody.textContent = message;
        toastEl.classList.remove("text-bg-success", "text-bg-danger", "text-bg-primary");
        if (type === "success") toastEl.classList.add("text-bg-success");
        else if (type === "error") toastEl.classList.add("text-bg-danger");
        else toastEl.classList.add("text-bg-primary");

        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }

    // Toggle views, catalogue or  upload)
    navLinks.forEach(link => {
        link.addEventListener("click", e => {
            e.preventDefault();
            navLinks.forEach(l => l.classList.remove("active"));
            link.classList.add("active");

            if (link.dataset.view === "catalogue") {
                catalogueView.classList.remove("d-none");
                uploadView.classList.add("d-none");
                loadCatalogue();
            } else {
                uploadView.classList.remove("d-none");
                catalogueView.classList.add("d-none");
            }
        });
    });

    // Load cataloge
    async function loadCatalogue() {
        const response = await fetch("/api/upload/list");
        const files = await response.json();

        const tbody = videoTable.querySelector("tbody");
        tbody.innerHTML = "";
        videoPlayerContainer.classList.add("d-none");
        videoPlayer.src = "";

        if (!files || files.length === 0) {
            videoTable.classList.add("d-none");
            catalogueMessage.classList.remove("d-none");
            return;
        }

        catalogueMessage.classList.add("d-none");
        videoTable.classList.remove("d-none");

        files.forEach(file => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td><a href="#">${file.fileName}</a></td>
                <td>${Math.round(file.size / 1024)}</td>
            `;
            tr.querySelector("a").addEventListener("click", e => {
                e.preventDefault();
                videoPlayer.src = file.url;
                videoPlayerContainer.classList.remove("d-none");
                videoPlayer.play();
            });
            tbody.appendChild(tr);
        });
    }

    // Upload form plus toast
    uploadForm.addEventListener("submit", async e => {
        e.preventDefault();

        const files = document.getElementById("fileInput").files;
        if (!files || files.length === 0) {
            showUploadToast("Please select at least one MP4 file before uploading.", "error");
            return;
        }

        const maxSize = 200 * 1024 * 1024;
        for (let f of files) {
            if (f.size > maxSize) {
                showUploadToast(`File "${f.name}" is too large. Max allowed size is 200 MB.`, "error");
                return;
            }
        }

        const formData = new FormData(uploadForm);
        try {
            const response = await fetch("/api/upload", { method: "POST", body: formData });

            if (response.ok) {
                showUploadToast("Upload successful!", "success");
                //switch back to catalogue after small delay so user can sees the toast
                setTimeout(() => {
                    document.querySelector('[data-view="catalogue"]').click();
                }, 300);
            } else {
                const err = await response.text();
                showUploadToast(`Upload failed: ${err || response.statusText}`, "error");
            }
        } catch (ex) {
            showUploadToast("Upload failed due to network error.", "error");
        }
    });

    // initial load
    document.querySelector('[data-view="catalogue"]').click();
});
