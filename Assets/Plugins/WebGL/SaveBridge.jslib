mergeInto(LibraryManager.library, {
    CommitInternalSave: function () {
        if (typeof FS !== 'undefined' && FS.syncfs) {
            FS.syncfs(false, function (err) {
                if (err) console.error("IndexedDB Sync Error: ", err);
                else console.log("Game Data Synced to Browser Storage!");
            });
        }
    }
});