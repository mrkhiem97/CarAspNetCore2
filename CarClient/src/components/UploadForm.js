import React, { Component } from 'react';
import $ from 'jquery';

class UploadForm extends Component {

    onClickTestAPI = () => {
        $.get("http://localhost:51080/api/Car", function (data, status) {
            alert("Data: " + JSON.stringify(data) + "\nStatus: " + status);
        });
    }

    onSubmit = (e) => {
        //prevent Default functionality
        e.preventDefault();
        //get the action-url of the form

        let fileUpload = $("#myfile").get(0);
        let files = fileUpload.files;
        let data = new FormData();
        for (let i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }

        $('input[type=text]').each((index, element) => {
            data.append(element.name, element.value);
        })

        //do your own request an handle the results
        $.ajax({
            //url: "http://localhost:51080/api/Car/UploadSmallFilesAjax",
            url: "http://localhost:51080/api/Car/UploadFileAsync",
            type: "POST",
            data: data,
            contentType: false,
            processData: false,

            xhr: function () {
                //upload Progress
                let xhr = $.ajaxSettings.xhr();
                if (xhr.upload) {
                    xhr.upload.addEventListener('progress', function (event) {
                        let percent = 0;
                        let position = event.loaded || event.position;
                        let total = event.total;
                        if (event.lengthComputable) {
                            percent = Math.ceil(position / total * 100);
                        }
                        //update progressbar
                        let txtPercent = percent + "%";
                        $("#percent").text(txtPercent);
                        $("#percent").css({ "width": txtPercent });
                        $("#percent").attr('aria-valuenow', percent);
                    }, true);
                }
                return xhr;
            },
            success: function (message) {
                $("#status").text(JSON.stringify(message));
            },
            error: function () {
                alert("There was error uploading files!");
            }
        });
    }

    render() {
        return (
            <form id="myform" method="post" className="form-horizontal" encType="multipart/form-data" onSubmit={e => this.onSubmit(e)}>
                <div className="form-inline">
                    <div className="form-group">
                        <table className="table table-borderless">
                            <tbody>
                                <tr>
                                    <td><label className="control-label">Model Id</label></td>
                                    <td><input type="text" name="modelid" value="123456" className="form-control form-control-sm" /></td>
                                    <td><label className="control-label">Model Name</label></td>
                                    <td><input type="text" name="modelname" value="Mazda CX 5" className="form-control form-control-sm" /></td>
                                    <td><label className="control-label">Model Version</label></td>
                                    <td><input type="text" name="version" value="7654" className="form-control form-control-sm" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div className="form-inline">
                    <div className="form-group">
                        <label className="control-label">Upload large file:</label>
                        <input type="file" className="form-control-file form-control-sm" id="myfile" multiple />
                    </div>
                </div>

                <div className="form-inline">
                    <div className="col-lg-3">
                        <button type="submit" className="btn btn-sm btn-default">Upload</button>
                        <button type="button" className="btn btn-sm btn-primary" onClick={this.onClickTestAPI}>Test API</button>
                    </div>
                    
                    <div className="col-lg-6">
                        <div className="progress">
                            <div id="percent" className="progress-bar progress-bar-success progress-bar-striped" role="progressbar" aria-valuenow="0" aria-valuemin="0"
                                aria-valuemax="100" style={{ width: 25 + '%' }}>
                                70%
                  </div>
                        </div>
                    </div>
                </div>
                <div id="status"></div>
            </form>
        );
    }
}

export default UploadForm;