$(document).ready(function () {
    //multiple file uploading
    $('#uploadBtn').click(function () {
        //event.preventDefault();
        //console.log($("#dllFileUploadForm").serialize());
        var filesData = new FormData();
        var files = $('form input[type=file]')[0].files;
        for (var i in files) {
            filesData.append(i, files[i]);
        }
        $.ajax({
            url: '/api/Files/',
            processData: false,
            contentType: false,
            data: filesData,
            type: 'POST'
        }).done(function (result) {
            filesModel.getUploadedFiles();
        }).fail(function (a, b, c) {
            $(".alert").show();
            $(".alert").add(a + b + c);
            console.log(a, b, c);
        });
        
    });

});