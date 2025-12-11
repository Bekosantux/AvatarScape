mergeInto(LibraryManager.library, {
    /**
     * ファイル選択ダイアログを開く
     * @param {string} gameObjectName - コールバックを受け取るGameObjectの名前
     * @param {string} methodName - コールバックメソッドの名前
     * @param {string} accept - 受け入れるファイル拡張子 (例: ".unity3d,.assetbundle")
     */
    OpenFileDialog: function(gameObjectNamePtr, methodNamePtr, acceptPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var methodName = UTF8ToString(methodNamePtr);
        var accept = UTF8ToString(acceptPtr);

        // 既存のinput要素を削除
        var existingInput = document.getElementById('avatarscape-file-input');
        if (existingInput) {
            existingInput.remove();
        }

        // input要素を作成
        var input = document.createElement('input');
        input.type = 'file';
        input.id = 'avatarscape-file-input';
        input.accept = accept;
        input.style.display = 'none';
        document.body.appendChild(input);

        input.onchange = function(e) {
            var file = e.target.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function(event) {
                    // ArrayBufferをBase64に変換
                    var arrayBuffer = event.target.result;
                    var bytes = new Uint8Array(arrayBuffer);
                    var binary = '';
                    for (var i = 0; i < bytes.byteLength; i++) {
                        binary += String.fromCharCode(bytes[i]);
                    }
                    var base64 = btoa(binary);
                    
                    // Unity側にコールバック
                    SendMessage(gameObjectName, methodName, base64);
                };
                reader.onerror = function() {
                    SendMessage(gameObjectName, 'OnFileLoadError', 'Failed to read file');
                };
                reader.readAsArrayBuffer(file);
            }
            input.remove();
        };

        input.click();
    },

    /**
     * ドラッグ&ドロップハンドラーをセットアップ
     * @param {string} gameObjectName - コールバックを受け取るGameObjectの名前
     */
    SetupDragAndDrop: function(gameObjectNamePtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var canvas = document.getElementById('unity-canvas');
        
        if (!canvas) {
            console.error('[AvatarScape] Unity canvas not found');
            return;
        }

        // ドラッグオーバー時のスタイル用オーバーレイ
        var overlay = document.getElementById('avatarscape-drop-overlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'avatarscape-drop-overlay';
            overlay.style.cssText = 'position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(100,150,255,0.3);display:none;pointer-events:none;z-index:9999;';
            overlay.innerHTML = '<div style="position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);font-size:24px;color:white;text-shadow:2px 2px 4px rgba(0,0,0,0.5);">Drop Avatar File Here</div>';
            document.body.appendChild(overlay);
        }

        // ドラッグイベント
        document.addEventListener('dragover', function(e) {
            e.preventDefault();
            e.stopPropagation();
            overlay.style.display = 'block';
        });

        document.addEventListener('dragleave', function(e) {
            e.preventDefault();
            e.stopPropagation();
            if (e.target === document || e.target === document.body) {
                overlay.style.display = 'none';
            }
        });

        document.addEventListener('drop', function(e) {
            e.preventDefault();
            e.stopPropagation();
            overlay.style.display = 'none';

            var files = e.dataTransfer.files;
            if (files.length > 0) {
                var file = files[0];
                var reader = new FileReader();
                reader.onload = function(event) {
                    var arrayBuffer = event.target.result;
                    var bytes = new Uint8Array(arrayBuffer);
                    var binary = '';
                    for (var i = 0; i < bytes.byteLength; i++) {
                        binary += String.fromCharCode(bytes[i]);
                    }
                    var base64 = btoa(binary);
                    SendMessage(gameObjectName, 'OnFileDropped', base64);
                };
                reader.onerror = function() {
                    SendMessage(gameObjectName, 'OnFileLoadError', 'Failed to read dropped file');
                };
                reader.readAsArrayBuffer(file);
            }
        });

        console.log('[AvatarScape] Drag and drop handler initialized');
    },

    /**
     * ダウンロードダイアログを表示（スクリーンショット保存用）
     * @param {string} filename - ファイル名
     * @param {string} base64Data - Base64エンコードされたデータ
     */
    DownloadFile: function(filenamePtr, base64DataPtr) {
        var filename = UTF8ToString(filenamePtr);
        var base64Data = UTF8ToString(base64DataPtr);

        var binary = atob(base64Data);
        var bytes = new Uint8Array(binary.length);
        for (var i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        var blob = new Blob([bytes], { type: 'application/octet-stream' });
        var url = URL.createObjectURL(blob);

        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        a.click();

        URL.revokeObjectURL(url);
    }
});
