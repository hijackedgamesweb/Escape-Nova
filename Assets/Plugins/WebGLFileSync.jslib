mergeInto(LibraryManager.library, {
  SyncFiles: function() {
    try {
      if (typeof FS !== 'undefined' && FS.syncfs) {
        FS.syncfs(false, function(err) {
          if (err) {
            console.log('WebGLFileSync: FS.syncfs error', err);
          } else {
            console.log('WebGLFileSync: FS.syncfs OK');
          }
        });
      } else {
        console.log('WebGLFileSync: FS.syncfs not available.');
      }
    } catch (e) {
      console.log('WebGLFileSync: exception', e);
    }
  }
});