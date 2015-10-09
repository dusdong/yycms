angular.module("app")
.factory("API_Common", function ($http, $q) {
        var Service = {};
        Service.url = "/api/Common/";
        Service.SiteConfig = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "SiteConfig", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_User", function ($http, $q) {
    var Service = {};
    Service.url = "/api/User/";
    Service.Get = function (PageIndex, PageSize, UserName, StartTime, EndTime, OrderBy, IsDesc, Role) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            UserName: UserName,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            Role: Role
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Login = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Login", data: user, headers: { 'Content-Type': 'application/json' } })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.LoginOut = function ()
    {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "LoginOut" })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_UserType", function ($http, $q) {
    var Service = {};
    Service.url = "/api/UserType/";
    Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Platform", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Platform/";
    Service.Get = function (PageIndex, PageSize, UserName, StartTime, EndTime, OrderBy, IsDesc, Role) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            UserName: UserName,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            Role: Role
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_News", function ($http, $q) {
    var Service = {};
    Service.url = "/api/News/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_NewsType", function ($http, $q) {
    var Service = {};
    Service.url = "/api/NewsType/";
    Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Product", function ($http, $q) {
        var Service = {};
        Service.url = "/api/Product/";
        Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title: Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowHide = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.HideByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_ProductType", function ($http, $q) {
    var Service = {};
    Service.url = "/api/ProductType/";
    Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Photo", function ($http, $q) {
        var Service = {};
        Service.url = "/api/Photo/";
        Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title: Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (entity, entity2) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: { Photo: entity, Items: entity2 } })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (entity, entity2) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: { Photo: entity, Items: entity2 } })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowHide = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.HideByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_PhotoType", function ($http, $q) {
        var Service = {};
        Service.url = "/api/PhotoType/";
        Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.HideByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
})
.factory("API_Video", function ($http, $q) {
        var Service = {};
        Service.url = "/api/Video/";
        Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title: Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowHide = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.HideByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_VideoType", function ($http, $q) {
        var Service = {};
        Service.url = "/api/VideoType/";
        Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.HideByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
})
.factory("API_Page", function ($http, $q) {
        var Service = {};
        Service.url = "/api/Page/";
        Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID, PageKind,CanBuild) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title:Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID,
                PageKind: PageKind,
                CanBuild:CanBuild
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Config = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Config", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.AddTask = function (typeid, ids) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "AddTask?typeid=" + typeid + "&ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.RetryTask = function (taskid) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "RetryTask?taskid=" + taskid })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };

        
        Service.Task_Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID, PageKind, PageID) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title: Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID,
                PageKind: PageKind,
                PageID: PageID
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Task_Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Build = function (pagecode) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Build", data: pagecode })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_PageType", function ($http, $q) {
        var Service = {};
        Service.url = "/api/PageType/";
        Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_Spider", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Spider/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_SpiderNews", function ($http, $q) {
        var Service = {};
        Service.url = "/api/SpiderNews/";
        Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
            var Data = {
                PageIndex: PageIndex,
                PageSize: PageSize,
                Title: Title,
                StartTime: StartTime,
                EndTime: EndTime,
                OrderBy: OrderBy,
                IsDesc: IsDesc,
                TypeID: TypeID
            };

            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Get", data: Data })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Detail = function (id) {
            var deferred = $q.defer();
            $http({ method: "GET", url: Service.url + "Get?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Post = function (user) {
            var deferred = $q.defer();
            $http({ method: "POST", url: Service.url + "Post", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Put = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "Put", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ShowHide = function (user) {
            var deferred = $q.defer();
            $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.Delete = function (id) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.DeleteByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        Service.ReleaseByIDs = function (ids) {
            var deferred = $q.defer();
            $http({ method: "HttpPost", url: Service.url + "ReleaseByIDs?ids=" + ids })
                .success(function (data, status, headers, config) { deferred.resolve(data); })
            return deferred.promise;
        };
        return Service;
    })
.factory("API_SinglePage", function ($http, $q) {
    var Service = {};
    Service.url = "/api/SinglePage/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Banner", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Banner/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_BannerType", function ($http, $q) {
    var Service = {};
    Service.url = "/api/BannerType/";
    Service.Get = function (PageIndex, PageSize, StartTime, EndTime, OrderBy, IsDesc) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (user) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_FriendLink", function ($http, $q) {
    var Service = {};
    Service.url = "/api/FriendLink/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (entity) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (entity) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (entity) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Keywords", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Keywords/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (entity) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (entity) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowHide = function (entity) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowHide", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.DeleteByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "DeleteByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Message", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Message/";
    Service.Get = function (PageIndex, PageSize, Title, StartTime, EndTime, OrderBy, IsDesc, TypeID) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            Title: Title,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            TypeID: TypeID
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Post = function (entity) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Post", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (entity) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Delete = function (id) {
        var deferred = $q.defer();
        $http({ method: "DELETE", url: Service.url + "Delete?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})
.factory("API_Wechat", function ($http, $q) {
    var Service = {};
    Service.url = "/api/Wechat/";
    Service.Get = function (PageIndex, PageSize, UserName, StartTime, EndTime, OrderBy, IsDesc, Role) {
        var Data = {
            PageIndex: PageIndex,
            PageSize: PageSize,
            UserName: UserName,
            StartTime: StartTime,
            EndTime: EndTime,
            OrderBy: OrderBy,
            IsDesc: IsDesc,
            Role: Role
        };

        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "Get", data: Data })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.News = function (_entity) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "News", data: _entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Detail = function (id) {
        var deferred = $q.defer();
        $http({ method: "GET", url: Service.url + "Get?id=" + id })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.RefreshFans = function () {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "RefreshFans" })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.Put = function (user) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "Put", data: user })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.MessageByIDs = function (entity) {
        var deferred = $q.defer();
        $http({ method: "POST", url: Service.url + "MessageByIDs", data: entity })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.ShowByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "ShowByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    Service.HideByIDs = function (ids) {
        var deferred = $q.defer();
        $http({ method: "PUT", url: Service.url + "HideByIDs?ids=" + ids })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
        return deferred.promise;
    };
    return Service;
})