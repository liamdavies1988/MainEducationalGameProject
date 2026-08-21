mergeInto(LibraryManager.library, {
    JS_SyncFileSystem: function () {
        if (typeof FS !== 'undefined' && FS.syncfs) {
            FS.syncfs(false, function (err) {
                if (err) console.error("IndexedDB Sync Error: ", err);
                else console.log("IndexedDB Sync Successful!");
            });
        }
    }
});