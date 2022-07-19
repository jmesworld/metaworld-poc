function getURLParameter(name) {
    //return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });
    // Get the value of "some_key" in eg "https://example.com/?some_key=some_value"
    return params[name];
}

function setURLParameter(name, value) {
    let params = new URLSearchParams(window.location.search);
    params.set(name, value);
    const nextURL = '?' + params.toString();
    const nextTitle = 'Tellyclub';
    const nextState = { additionalInformation: 'changed ' + name + ' to ' + value };

    // This will create a new entry in the browser's history, without reloading
    window.history.replaceState(nextState, null, nextURL);
}