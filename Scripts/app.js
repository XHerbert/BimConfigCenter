/// <reference path="jquery-3.3.1.min.js" />

var integrate_filelist = [];
var render_filelist = [];
var integrate_data;
var configdata;
var descDom = $("tr#desc");
var host = "http://10.0.197.82:8078/api/";


// Get list of model files
document.getElementById("query").addEventListener("click", function () {
    let val = document.getElementById("proj").value;
    if (!val || val === "-1") {
        // query all of upload history
        let all_option = {
            url: host + "file/getUploadFileList",
            type: "GET",
            data: {},
            cache: false,
            success: function (response) {
                console.log(response);
                if (!response || response.data.length === 0) return;
                let size = response.data.length;

                let lis = '';
                for (let i = 0; i < size; i++) {
                    let current_li = '<li class=""><span>fileId:</span>' + response.data[i].fileId + '<br>' + response.data[i].name +
                        '<a id="' + response.data[i].fileId + '" onclick = "addThis(this)" data-added=false data-id="' + response.data[i].fileId + '" data-floor="" data-spe=""  data-name="' + response.data[i].name + '" class="btn-add">添加</a></li>';
                    lis += current_li;
                }
                $("#models").empty();
                $("#models").html(lis);
            }
        };
        $.ajax(all_option);
        return;
    }

    let data = { project: val };
    let option = {
        url: "/Operation/FetchModelFiles",
        type: "GET",
        data: data,
        cache: false,
        success: function (response) {
            if (!response || response.length === 0) return;
            let size = response.length;

            let lis = '';
            for (let i = 0; i < size; i++) {
                let current_li = '<li class=""><span>fileId:</span>' + response[i].FileId + '<br>' + response[i].Name +
                    '<a id="' + response[i].FileId + '" onclick = "addThis(this)" data-added=false data-id="' + response[i].FileId + '" data-floor="' + response[i].Floor + '" data-spe="' + response[i].Specialty + '"  data-name="' + response[i].Name + '" class="btn-add">添加</a></li>';
                lis += current_li;
            }
            $("#models").empty();
            $("#models").html(lis);
        }
    };

    $.ajax(option);
});

// integrate button
document.getElementById("integrate").addEventListener('keyup', function () {
    let inputVal = $("#integrate").val();
    if (inputVal.trim()) {
        $("#beginbtn").removeClass("bee-button-disabled");
    } else {
        $("#beginbtn").addClass("bee-button-disabled");
    }
});

// clear file items
document.getElementById('clear').addEventListener("click", function () {
    $('#integs').html('');
    $('#integs').append(descDom);
    $("tr#desc").css("display", "block");
    calcFileCount(0);
});


// send integrate
document.getElementById("beginbtn").addEventListener("click", function () {
    // verify data

    console.log(integrate_filelist);
    let inputs_spe = $('#integs input[name="specialty"]');
    if (inputs_spe.length === 0) {
        layer.alert('请添加模型', {
            skin: 'layui-layer-molv',
            closeBtn: 0
        });
        return;
    }

    // prepare data

    integrate_filelist = [];
    let tr_collection = $('#integs tr');
    let tr_length = tr_collection.length;
    for (let o = 0; o < tr_length; o++) {
        let current = tr_collection[o];
        if (current.id === 'desc') continue;
        let fileId = current.getAttribute('data-fileId');
        let floor = current.children[2].children[0].value;
        let spe = current.children[1].children[0].value;

        // add params
        let data = { "fileId": Number(fileId), "floor": floor, "specialty": spe };
        integrate_filelist.push(data);
    }

    let integrate_name = document.getElementById("integrate").value;
    let config = document.getElementById("config");
    let selectedIndex = config.selectedIndex;
    let selectedValue = config.options[selectedIndex].value;
    if (selectedValue === "1") {
        configdata = "{\"loadOnDemand\":true,\"fileType\": \"rvt\"}";
        integrate_data = {
            sources: integrate_filelist,
            name: integrate_name,
            config: configdata,
            priority: 2
        };
    } else {
        integrate_data = {
            sources: integrate_filelist,
            name: integrate_name,
            priority: 2
        };
    }

    for (let i = 0, len = inputs_spe.length; i < len; i++) {
        let _ = inputs_spe[i].getAttribute("data-file");
        for (var j = 0; j < len; j++) {
            let __ = integrate_filelist[j].fileId.toString();
            if (__ === _) {
                integrate_filelist[j].specialty = inputs_spe[i].value;
            }
        }
    }
    // send request

    let options = {
        url: host + "integrate/integrateModels",
        type: "PUT",
        data: JSON.stringify(integrate_data),
        contentType: 'application/json',
        cache: false,
        success: function (response) {
            if (response.code === 20000) {
                layer.alert('模型集成成功', {
                    skin: 'layui-layer-molv'
                    , closeBtn: 0
                }, function () {
                    window.location.reload(true);
                });
            } else {
                layer.alert(response.message, {
                    skin: 'layui-layer-molv'
                    , closeBtn: 0
                });
            }
        }
    };
    $.ajax(options);
});

// query history of integrate
document.getElementById("queryHistory").addEventListener("click", function () {

    // get data
    let options = {
        url: host + "integrate/integrateHistory",
        type: "POST",
        cache: false,
        success: function (response) {
            if (!response || response.data.length === 0) return;
            let size = response.data.length;

            let li_history = '';
            for (let i = 0; i < size; i++) {
                let item = response.data[i];
                let current_li = '';
                current_li += '<li>';
                current_li += '  <ul class="history_m">';
                current_li += '    <li style="width:33%">' + item.name + '</li>';
                current_li += '    <li>' + item.integrateId + '</li>';
                current_li += '    <li>' + item.createTime + '</li>';
                current_li += '    <li style="width:8%">' + item.cost + 's</li>';
                current_li += '    <li style="color: rgb(50, 211, 166);width:8%">' + item.status + '</li>';
                current_li += '  </ul >';
                current_li += '<div class="cz"><i title="载入" onclick="loadSubFiles(this)" data-integrate=' + item.integrateId + ' class="iconfont icon-up1" style="font-size: 14px; color: rgb(50, 211, 166);"></i></div></li > ';
                li_history += current_li;
            }
            $("ul.options").empty();
            $("ul.options").html(li_history);

            // show dialog
            $("div.bee-modal-mask").css("display", "block");
            document.getElementById("his").style.display = "block";
        }
    };

    // send request
    $.ajax(options);
});


// filter by specialty
document.getElementById('filter_specialty').addEventListener("click", function () {
    let file_count = calcFileCount();
    if (file_count === 0) {
        layer.msg('请选择参与集成的文件列表', { icon: 5 });
        return;
    }
    //获取列表中所有的专业
    let specialtyArray = [];
    let domHtml = '';
    for (let o = 0; o < render_filelist.length; o++) {
        let val = render_filelist[o].specialty;
        if (specialtyArray.indexOf(val) === -1) {
            specialtyArray.push(val);
            domHtml += '<li class="list-group-item" style="margin-right:15px;margin-left:15px">';
            domHtml += '<div class="has-success">';
            domHtml += '<div class="checkbox">';
            domHtml += '<label><input type="checkbox" id="checkboxSuccess" name="f_specialty" value="' + val + '">';
            domHtml += val;
            domHtml += '</label></div></div></li>';
        }
    }
    $('#filter_spe_panel_ul').html('');
    $('#filter_spe_panel_ul').html(domHtml);
    layer.open({
        type: 1,
        title: '按专业过滤',
        area: ['400px', '280px'],
        content: $("#filter_spe_panel"),
        btn: ['确认', '取消'],
        //下标1开始：index，layero当前弹窗对象
        yes: function (index, layero) {
            console.log(specialtyArray);
            let _condition = [];
            $('input[name="f_specialty"]').each(item => {
                if ($('input[name="f_specialty"]')[item].checked) {
                    _condition.push($('input[name="f_specialty"]')[item].value);
                }
            });
            // 重新渲染当前列表
            filterByCondition(render_filelist, _condition, false);
            layer.close(index);
        }
    });
});


// filter by floor
document.getElementById('filter_floor').addEventListener("click", function () {
    let file_count = calcFileCount();
    if (file_count === 0) {
        layer.msg('请选择参与集成的文件列表', { icon: 5 });
        return;
    }
    //获取列表中所有的专业
    let floorArray = [];
    let domHtml = '';
    for (let o = 0; o < render_filelist.length; o++) {
        let val = render_filelist[o].floor;
        if (floorArray.indexOf(val) === -1) {
            floorArray.push(val);
            domHtml += '<li class="list-group-item" style="margin-right:15px;margin-left:15px">';
            domHtml += '<div class="has-success">';
            domHtml += '<div class="checkbox">';
            domHtml += '<label><input type="checkbox" name="f_floor" id="checkboxSuccess" value="' + val + '">';
            domHtml += val;
            domHtml += '</label></div></div></li>';
        }
    }
    $('#filter_floor_panel_ul').html('');
    $('#filter_floor_panel_ul').html(domHtml);

    layer.open({
        type: 1,
        title: '按楼层过滤',
        area: ['400px', '280px'],
        content: $("#filter_floor_panel"),
        btn: ['确认', '取消'],
        yes: function (index, layero) {
            console.log(floorArray);
            let _condition = [];
            let $f_floor = $('input[name="f_floor"]');
            $f_floor.each(item => {
                if ($f_floor[item].checked) {
                    _condition.push($f_floor[item].value);
                }
            });
            // 重新渲染当前列表
            filterByCondition(render_filelist, _condition, true);
            layer.close(index);
        }
    });
});

// add items
function addThis(obj) {
    if (obj.getAttribute('data-added') === "true") return;

    // todo repeat filter
    $("tr#desc").css("display", "none");
    let fileId = obj.getAttribute('data-id');
    let name = obj.getAttribute('data-name');
    let floor = obj.getAttribute('data-floor');
    let spe = obj.getAttribute('data-spe');

    // add params
    let data = { "fileId": Number(fileId), "floor": floor, "specialty": spe };
    integrate_filelist.push(data);

    let tr = '<tr class="tr-index" data-fileId="' + fileId + '"><td><span class="name">fileId:</span>' + fileId + '<br>' + name + '</td>';
    tr += '<td width="150">';
    tr += '<input type="text" data-file="' + fileId + '" value="' + spe + '" class="bee-input-input bee-input-medium" style="width:198px" name="specialty" /></td>';

    tr += '<td width="150">';
    tr += '<input type="text" data-file="' + floor + '" value="' + floor + '" class="bee-input-input bee-input-medium" style="width:198px" name="floor" /></td>';

    tr += '<td width="80"><a style="cursor:pointer" onclick="removeThis(this)" data-id="' + fileId + '">移除</a></td></tr>';
    obj.innerText = '已添加';
    obj.setAttribute('data-added', true);
    obj.style.color = 'red';
    $('table#integs').append(tr);
    render_filelist.push({ fileId: Number(fileId), fileName: name, floor: floor, specialty: spe });
    calcFileCount();
}

// remove items
function removeThis(obj) {
    let id = obj.getAttribute('data-id');
    let domId = '#' + id;
    $(domId).text('添加');
    $(obj).closest('tr').remove();
    $(domId).css('color', '#57eac1');
    $(domId).attr('data-added', false);
    if (integrate_filelist.length !== render_filelist.length) {
        layer.msg("数据不一致");
        return;
    }
    for (let i = 0, len = integrate_filelist.length; i < len; i++) {
        if (id === integrate_filelist[i].fileId.toString()) {
            integrate_filelist.splice(i, 1);
        }
    }
    for (let i = 0, len = render_filelist.length; i < len; i++) {
        if (id === render_filelist[i].fileId.toString()) {
            render_filelist.splice(i, 1);
        }
    }
    if ($('#integs input').length === 0) {
        $("tr#desc").css("display", "block");
    }
    calcFileCount();
}

// load sub files
function loadSubFiles(load) {
    let integrateId = Number(load.getAttribute("data-integrate"));
    if (!integrateId) return;

    let options = {
        url: host + "integrate/integrateSubFiles?integrateId=" + integrateId,
        type: "GET",
        cache: false,
        success: function (response) {
            //载入后左侧列表恢复
            render_filelist = [];
            renderSubFiles(response.data);
            response.data.forEach((item, index) => {
                render_filelist.push(item);
            });
            console.log("render_filelist", render_filelist);
        }
    };

    // send request
    $.ajax(options);
}

// reder sub files
function renderSubFiles(fileArray) {
    $("tr#desc").css("display", "none");
    let respo = fileArray;
    let size = respo.length;
    $('table#integs').empty();
    for (let k = 0; k < size; k++) {
        let resp = fileArray[k];
        let data = { "fileId": Number(resp.fileId), "floor": resp.floor, "specialty": resp.specialty };

        let tr = '<tr class="tr-index" data-fileId="' + resp.fileId + '" data-spe="' + resp.specialty + '" data-floor="' + resp.floor + '"><td><span class="name">fileId:</span>' + resp.fileId + '<br>' + resp.fileName + '</td>';
        tr += '<td width="150">';
        tr += '<input type="text"  data-file="' + resp.fileId + '" value="' + resp.specialty + '" class="bee-input-input bee-input-medium" style="width:198px" name="specialty" /></td>';
        tr += '<td width="150">';
        tr += '<input type="text"  data-file="' + resp.floor + '" value="' + resp.floor + '" class="bee-input-input bee-input-medium" style="width:198px" name="floor" /></td>';
        tr += '<td width="80"><a style="cursor:pointer" onclick="removeThis(this)" data-id="' + resp.fileId + '">移除</a></td></tr>';
        $('table#integs').append(tr);
    }
    calcFileCount();
    // close current dialog
    $("#his").css("display", "none");
    $("div.bee-modal-mask").css("display", "none");
}


// calculate integrate files
function calcFileCount(count) {
    if (count !== undefined) {
        $('#fct').text("0");
        return;
    }
    let ct = $('.tr-index').length;
    $('#fct').text(ct);
    return ct;
}

// render list
function filterByCondition(arrayData, conditionArr, isFloor) {
    if (!conditionArr || conditionArr.length === 0) {
        renderSubFiles(render_filelist);
        return;
    }
    let result = [];
    for (let i = 0; i < conditionArr.length; i++) {
        for (let j = 0; j < arrayData.length; j++) {
            if (isFloor) {
                if (arrayData[j].floor === conditionArr[i]) {
                    result.push(arrayData[j]);
                }
            } else {
                if (arrayData[j].specialty === conditionArr[i]) {
                    result.push(arrayData[j]);
                }
            }
        }
    }
    renderSubFiles(result);
}

