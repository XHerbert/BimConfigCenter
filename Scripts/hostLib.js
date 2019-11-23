/**
 * 统一配置IP地址以及接口地址
 * 以及常用常量
 **/

var ip = 'http://10.1.73.67:8079';
var hostAddr = {
    system_config: ip + '/api/system/config/',
    scene: ip + '/api/scene/',
    authority: ip + '/api/appkey/',
    equPropGroup: ip +'/api/equPropGroup/'
};

var method = {
    GET: "GET",
    POST: "POST"
};
