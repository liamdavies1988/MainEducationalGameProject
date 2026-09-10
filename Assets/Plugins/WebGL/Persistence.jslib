mergeInto(LibraryManager.library, {
    CommitInternalSave__deps: ['$FS'],
    CommitInternalSave: function () {
        if (typeof FS !== 'undefined' && FS.syncfs) {
            FS.syncfs(false, function (err) {
                if (err) {
                    console.error("IndexedDB Sync Error: ", err);
                } else {
                    console.log("Game Data Synced to Browser Storage!");
                }
            });
        }
    },

    JS_SyncFileSystem__deps: ['$FS'],
    JS_SyncFileSystem: function () {
        if (typeof FS !== 'undefined' && FS.syncfs) {
            FS.syncfs(false, function (err) {
                if (err) {
                    console.error("IndexedDB Sync Error: ", err);
                } else {
                    console.log("IndexedDB Sync Successful!");
                }
            });
        }
    }
});