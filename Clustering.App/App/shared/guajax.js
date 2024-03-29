﻿define([], function() {
    function guajax() {
        var exports = this,
        requests = [],
        counter = 0;



        exports.getWithAbortPrevious = function(url, data, identifier) {
            return exports.ajax({
                type: "GET",
                url: url,
                data: data,
                isSingle: true,
                identifier: identifier
            });
        };

        exports.get = function(url, data, allowFail) {
            return exports.ajax({
                type: "GET",
                url: url,
                data: data,
                allowFail: allowFail
            });
        };

        exports.put = function(url, data) {
            return exports.ajax({
                type: "PUT",
                url: url,
                data: data
            });
        };

        exports.patch = function(url, data) {
            return exports.ajax({
                type: "PATCH",
                url: url,
                data: data
            });
        };

        exports.del = function(url, data) {
            if(data) {
                return exports.ajax({
                    type: "DELETE",
                    url: url,
                    data: data
                });
            } else {
                return exports.ajax({
                    type: "DELETE",
                    url: url
                });
            }
        };

        exports.post = function(url, data) {
            return exports.ajax({
                type: "POST",
                url: url,
                data: data
            });
        };

        exports.postFile = function(url, data) {
            return exports.xhr({
                type: "POST",
                url: url,
                data: data
            });
        };

        exports.xhr = function(options) {
            var deferred = Q.defer();

            var request = new guajaxRequest(deferred, options);

            runXhrRequest(request);

            return deferred.promise;
        };

        exports.ajax = function(options) {
            var deferred = Q.defer();

            var request = new guajaxRequest(deferred, options);

            if(request.isSingle) {
                var identifier = request.identifier || options.url;
                abortAndRemovePreviousRequest(identifier);
            }

            requests.push(request);
            runRequest(request);

            return deferred.promise;
        };

        exports.hasUnauthorizedRequest = function() {
            for(var i = 0; i < requests.length; i++) {
                if(requests[i].jqueryAjax.status == 401) {
                    return true;
                }
            }
            return false;
        };

        exports.resubmitUnauthorizedRequests = function() {
            for(var i = 0; i < requests.length; i++) {
                if(requests[i].jqueryAjax.status == 401) {
                    runRequest(requests[i]);
                }
            }
        };

        function guajaxRequest(deferred, options) {
            return {
                id: getNextRequestId(),
                deferred: deferred,
                options: options,
                isSingle: options.isSingle || false,
                identifier: options.identifier || options.url,
                allowFail: options.allowFail || false
            };
        }

        function findIndexOf(callback) {
            for(var i = 0; i < requests.length; i++) {
                if(callback(requests[i]))
                    return i;
            }

            return -1;
        }

        function abortAndRemovePreviousRequest(identifier) {
            var index = findIndexOf(function(request) {
                return (request.identifier === identifier);
            });

            if(index > -1) {
                requests[index].jqueryAjax.aborted = true;
                requests[index].jqueryAjax.abort();
                requests.splice(index, 1);
            }
        }

        function getNextRequestId() {
            return "request_" + (++counter);
        }

        function removeRequestById(id) {
            var index = findIndexOf(function(request) {
                return (request.id === id);
            });

            requests.splice(index, 1);
        }

        function runRequest(request) {
            request.jqueryAjax = $.ajax(request.options)
            .done(function(response) {
                request.deferred.resolve(response);
                removeRequestById(request.id);
            })
            .fail(function(jqXHR) {
                // 401s are queued up and retried when call resubmitUnauthorizedRequests
                if(jqXHR.status !== 401) {
                    request.deferred.reject(jqXHR.responseJSON);
                    removeRequestById(request.id);
                }
            });
        }

        function runXhrRequest(request) {
            var xhrRequest = new XMLHttpRequest();
            var formData = new FormData();

            xhrRequest.open(request.options.type, request.options.url);

            xhrRequest.onload = function() {
                var response = JSON.parse(xhrRequest.response);

                if(xhrRequest.status == 200) {
                    request.deferred.resolve(response);
                } else {
                    request.deferred.reject(response);
                }
            };

            for(var key in request.options.data) {
                formData.append(key, request.options.data[key]);
            }

            xhrRequest.send(formData);
        }
    }

    return new guajax();
});