﻿var dataTableT;
$(document).ready(function () {
    loadDataTableTask();
});

//Need an api method return json to use this
function loadDataTableTask() {
    dataTableT = $('#tblTask').DataTable({
        "ajax": { url: '/Manager/Task/GetAll' },
        "columns": [
            {
                data: "id",
                "render": function (data) {
                    return `<a class="text-main text-pointer" onClick="ShowTaskDetail('/Engineer/Task/Detail?TaskId=${data}')" >${data}</a>`
                },
            },
            { data: 'name', },
            { data: 'description', },
            { data: 'unitPrice', },
            { data: 'category.name', },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group">
                       <a href='/Manager/Task/Edit?id=${data}' class="btn btn-primary btn-main border-0 m-1"><i class="bi bi-pencil"></i> Chỉnh sửa </a>
                       <a href='/Manager/Task/Delete?id=${data}' class="btn btn-danger border-0 m-1"><i class="bi bi-trash"></i> Xóa </a>
                    </div>`
                },
            }
        ]
    });
}
