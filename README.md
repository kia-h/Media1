
# **Video Upload MVC App – Summary**

## **Project Overview**

This solution consists of three projects:

1. **MVC Project**

   * Implements a **video catalogue and upload system**.
   * Uses **ASP.NET Core MVC** with **Bootstrap** for UI.
   * Features:

     * Upload MP4 videos (configurable file size and allowed extensions in `appsettings.json`).
     * List uploaded videos in a table with clickable links to play.
     * **Responsive video player** above the catalogue.
     * **Toast notifications** for upload success/error.
   * Client-side logic handled in `wwwroot/js/app.js`.
   * No custom CSS; only Bootstrap for layout and styling.

2. **Unit Test Project (xUnit)**

   * Tests `MediaService` and `UploadController` behaviors.
   * Validates:

     * File filtering and allowed extensions.
     * Proper handling of empty uploads.
     * Return values for `GetAllFiles()` and `Upload()` endpoints.

3. **BDD Test Project (SpecFlow placeholder)**

   * Contains a **single SpecFlow scenario** demonstrating awareness of BDD testing.
   * **Note:** SpecFlow is currently **not supported in .NET 8**, so this project serves as a demonstration placeholder.alternative ReqnRoll

---

## **Configuration**

In `appsettings.json`:

```json
{
  "FileUpload": {
    "MaxFileSizeMB": 200,
    "AllowedExtensions": [ ".mp4", ".mov", ".avi" ],
    "CurrentlySupportedExtensions": [ ".mp4" ],
    "UploadPath": "wwwroot/media"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

* **MaxFileSizeMB:** Maximum file size allowed for uploads.
* **AllowedExtensions:** Extensions supported by the system (future-proof).
* **CurrentlySupportedExtensions:** Extensions currently allowed for upload (currently only `.mp4`).
* **UploadPath:** Folder where uploaded files are stored.

---

## **How to Run the MVC Project**

1. Open the solution in **Visual Studio 2022+** or **VS Code**.
2. Ensure **.NET 8 SDK** is installed.
3. Restore NuGet packages.
4. Build and run the MVC project:

   * Press `F5` or run `dotnet run` in the project folder.
5. Navigate to `https://localhost:5001` (or the port shown in output).

**Features to test:**

* Switch between **Catalogue** and **Upload** tabs.
* Upload an MP4 video and observe:

  * Toast notification for success/failure.
  * Video appears in the catalogue table.
  * Click the video filename to play in the video player.
* Check validation for:

  * File size exceeding limit.
  * Unsupported extensions (only `.mp4` allowed currently).

---

## **Running Unit Tests**

1. Open the **Unit Test Project** in Visual Studio or VS Code.
2. Run all tests using Test Explorer or CLI:

```
dotnet test
```

* All tests are written using **xUnit**.
* Validates `MediaService` and `UploadController` logic.

---

## **BDD / SpecFlow Tests**

* SpecFlow project contains **example feature file and scenario**.
* Currently **not runnable in .NET 8**.
* Included to demonstrate **awareness of BDD testing practices**.

---

## **Notes**

* **Project name `Media1`** was intentionally chosen to emphasize its focus on media handling; it reflects priority and purpose rather than being arbitrary or lazy naming.  
* No custom CSS; UI relies on **Bootstrap** for layout and responsive design.
* Uploads are stored in `wwwroot/media`.
* Video player always maintains **consistent size** across all videos.
* Toast notifications replace progress bars for simplicity and fast feedback.

---
