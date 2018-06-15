$(function () {
    function initEditor() {
        window.editor = ace.edit("editor");
        editor.session.setMode("ace/mode/json");
        editor.getSession().setTabSize(2);
        editor.getSession().setUseWrapMode(true);
    }

    initEditor();

    $('#stop-agent-btn').click(function () {
        var agentId = document.getElementById('Id').value;
        startSpin();
        $.ajax({
            url: "/Agents/Stop/",
            data: JSON.stringify({ id: agentId }),
            type: 'post',
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.msg !== "Готово")
                    alert(data.msg);
                stopSpin();
                location.reload();
            }
        });
    });

    $('#start-agent-btn').click(function () {
        var agentId = document.getElementById('Id').value;
        startSpin();
        $.ajax({
            url: "/Agents/Start/",
            data: JSON.stringify({ id: agentId }),
            type: 'post',
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.msg !== "Готово")
                    alert(data.msg);
                stopSpin();
                location.reload();
            }
        });
    });

    $('#sync-btn').click(function () {
        var agentId = document.getElementById('Id').value;
        startSpin();
        $.ajax({
            url: "/Agents/Sync/",
            data: JSON.stringify({
                id: agentId,
                jsonConfig: editor.getValue()
            }),
            type: 'post',
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.msg !== "Готово")
                    alert(data.msg);
                stopSpin();
                location.reload();
            }
        });
    });
});