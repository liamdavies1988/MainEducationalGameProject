mergeInto(LibraryManager.library, {
    // Method syncs virtual file system with browser IndexedDB
    SyncFilesystem: function () {
        if (typeof FS !== 'undefined' && FS.syncfs) {
            FS.syncfs(false, function (err) {
                if (err) {
                    console.warn("IndexedDB sync failed: ", err);
                }
            });
        }
    }
});