// This is a simple *viewmodel* - JavaScript that defines the data and behavior of your UI
function AppViewModel() {
    var self = this;
    self.uploads = ko.observableArray([]);
    self.error = ko.observable();
    var uploadFilesUri = "/api/Files/";
    var restoreFilesUri = "/api/Files/restore/";



    ajaxHelper = function (uri, method, data) {
        self.error(''); // Clear error message
        $(".alert").hide();
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : JSON.stringify(null)
        }).fail(function (jqXHR, textStatus, errorThrown) {
            $(".alert").show();
            self.error(errorThrown + " " + jqXHR.responseText);
        });
    };

    //GET files
    function getUploadedFiles() {
        ajaxHelper(uploadFilesUri, 'GET').done(function (data) {
            self.uploads(data);
        });
    }

    getUploadedFiles();


    //DELETE files
    self.deleteFile = function (item) {
        var dllFileName = item.dllFileName;
        //dllFileName = "CalcOperator.Parenthesis.Left.dll";
        ajaxHelper(uploadFilesUri + dllFileName, 'DELETE')
        .done(function (data) {
            getUploadedFiles();//
        });
    };


    //RESTORE deleted files
    //btnRestore
    self.restoreFile = function () {
        console.log("data", this);
        ajaxHelper(restoreFilesUri, 'POST')
        .done(function (data) {
            getUploadedFiles();//
            console.log(data);
        });
    };
}

// Activates knockout.js
var filesModel = new AppViewModel();
ko.applyBindings(filesModel);

