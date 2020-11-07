function confirmDelete(uniqeId, isDeleteClicked) {
    var deleteSpan = "deleteSpan_" + uniqeId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + uniqeId;

    if (isDeleteClicked) {
        $("#" + deleteSpan).hide();
        $("#" + confirmDeleteSpan).show();
    } else {
        $("#" + deleteSpan).show();
        $("#" + confirmDeleteSpan).hide();
    }
}