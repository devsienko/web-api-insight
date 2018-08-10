$(function () {
    $('#delete-user-btn').click(function () {
        var checkedItems = $(".checkbox-col input:checked");
        if (checkedItems.length == 0) {
            alert('You need to choose at least one element.');
            return;
        }
        else {
            var isConfirmed = confirm("Do you want to delete the selected items?");
            if (isConfirmed)
                deleteItems();
        }
    });

    $('#check-all-chck').click(function () {
        if ($(this).is(':checked')) {
            $(".checkbox-col input").prop('checked', true);
        } else {
            $(".checkbox-col input").prop('checked', false);
        }
    });

    function deleteItems() {
        var items = [];
        $(".checkbox-col input:checked").each(function () {
            items.push($(this).val());
        });
        console.log(items);
        startSpin();
        $.ajax({
            url: "Delete",
            data: JSON.stringify(items),
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
    }
});