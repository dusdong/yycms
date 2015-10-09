angular.module("app").controller("GlobalCtrl", function ($location, $scope) {

    $scope.PlatformUser = APP.User();

    $scope.LoginOut = function ()
    {
        if (!confirm("是否确认退出系统？")) { return;}
        $.removeCookie('session');
        $location.path("/login");
    };


    /*监视分页组件的当前页，如果有变动就调用分页方法*/
    $scope.$watch('Pager.PageIndex', function (newVal, oldVal) {
        if (newVal == undefined || oldVal == undefined) { return; }
        $scope.IData_Get();
    });

    $scope.config3 = {
        data: [{ id: 1, text: 'bug' }, { id: 2, text: 'duplicate' }, { id: 3, text: 'invalid' }, { id: 4, text: 'wontfix' }]
        // 其他配置略，可以去看看内置配置中的ajax配置
    };
});