mergeInto(LibraryManager.library, {

    // 显示HTML输入框
    // targetGameObject: 回调的Unity GameObject名称
    ShowMobileInput: function(placeholder, existingText, targetGameObject) {
        var ph = UTF8ToString(placeholder);
        var existing = UTF8ToString(existingText);
        var target = UTF8ToString(targetGameObject);

        // 如果已存在则移除
        var old = document.getElementById('unity-chinese-input');
        if (old) old.remove();
        var oldOverlay = document.getElementById('unity-input-overlay');
        if (oldOverlay) oldOverlay.remove();

        var canvas = document.querySelector('#unity-canvas') || document.querySelector('canvas');
        if (!canvas) return;

        var rect = canvas.getBoundingClientRect();

        // 创建半透明遮罩（防止Unity误触）
        var overlay = document.createElement('div');
        overlay.id = 'unity-input-overlay';
        overlay.style.cssText = [
            'position:fixed',
            'left:' + rect.left + 'px',
            'top:' + rect.top + 'px',
            'width:' + rect.width + 'px',
            'height:' + rect.height + 'px',
            'z-index:9998',
            'background:transparent'
        ].join(';');

        // 创建输入容器
        var container = document.createElement('div');
        container.id = 'unity-chinese-input';
        container.style.cssText = [
            'position:fixed',
            'left:50%',
            'top:50%',
            'transform:translate(-50%,-50%)',
            'z-index:9999',
            'background:rgba(30,30,30,0.92)',
            'border-radius:12px',
            'padding:24px 20px 20px 20px',
            'box-shadow:0 4px 30px rgba(0,0,0,0.5)',
            'min-width:380px',
            'max-width:90vw',
            'font-family:sans-serif'
        ].join(';');

        var label = document.createElement('div');
        label.textContent = ph || '请输入内容';
        label.style.cssText = 'color:#ccc;font-size:13px;margin-bottom:10px;';

        var input = document.createElement('input');
        input.type = 'text';
        input.value = existing || '';
        input.style.cssText = [
            'width:100%',
            'box-sizing:border-box',
            'padding:10px 12px',
            'font-size:18px',
            'border:2px solid #4a9',
            'border-radius:8px',
            'outline:none',
            'background:#1a1a1a',
            'color:#fff',
            'caret-color:#4a9',
            'margin-bottom:12px'
        ].join(';');

        var btnRow = document.createElement('div');
        btnRow.style.cssText = 'display:flex;gap:10px;justify-content:flex-end;';

        var cancelBtn = document.createElement('button');
        cancelBtn.textContent = '取消';
        cancelBtn.style.cssText = [
            'padding:8px 20px',
            'font-size:15px',
            'border:none',
            'border-radius:6px',
            'background:#555',
            'color:#fff',
            'cursor:pointer'
        ].join(';');

        var confirmBtn = document.createElement('button');
        confirmBtn.textContent = '确定';
        confirmBtn.style.cssText = [
            'padding:8px 20px',
            'font-size:15px',
            'border:none',
            'border-radius:6px',
            'background:#4a9',
            'color:#fff',
            'cursor:pointer'
        ].join(';');

        function confirm() {
            var val = input.value;
            cleanup();
            if (typeof SendMessage !== 'undefined') {
                SendMessage(target, 'OnWebGLInputConfirm', val);
            }
        }

        function cancel() {
            cleanup();
            if (typeof SendMessage !== 'undefined') {
                SendMessage(target, 'OnWebGLInputCancel', '');
            }
        }

        function cleanup() {
            var c = document.getElementById('unity-chinese-input');
            var o = document.getElementById('unity-input-overlay');
            if (c) c.remove();
            if (o) o.remove();
        }

        confirmBtn.onclick = confirm;
        cancelBtn.onclick = cancel;

        input.addEventListener('keydown', function(e) {
            // 阻止所有键盘事件冒泡到Unity，修复退格键/方向键被Unity拦截的问题
            e.stopPropagation();
            if (e.key === 'Enter' && !e.isComposing) {
                e.preventDefault();
                confirm();
            }
            if (e.key === 'Escape') {
                cancel();
            }
        });

        input.addEventListener('keyup', function(e) { e.stopPropagation(); });
        input.addEventListener('keypress', function(e) { e.stopPropagation(); });

        btnRow.appendChild(cancelBtn);
        btnRow.appendChild(confirmBtn);
        container.appendChild(label);
        container.appendChild(input);
        container.appendChild(btnRow);

        document.body.appendChild(overlay);
        document.body.appendChild(container);

        // 延迟聚焦，确保输入法能唤起
        setTimeout(function() {
            input.focus();
            input.setSelectionRange(input.value.length, input.value.length);
        }, 100);
    },

    // 关闭HTML输入框
    HideMobileInput: function() {
        var c = document.getElementById('unity-chinese-input');
        var o = document.getElementById('unity-input-overlay');
        if (c) c.remove();
        if (o) o.remove();
    }

});
