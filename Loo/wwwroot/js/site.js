// Write your JavaScript code.
function progressbar(SensorID, SensorData) {
    var sensorType = str.slice(sensortype[0]);

    var sensordata = sensordata1;
    if (sensorType == 'W')
        if (sensordata == 0) {
            window.alert("width: 0%");
            return ("width: 0%");
        }
        else {
            window.alert("width: 100%");
            return ("width: 100%");
        }
    else if (sensorType == 'R') {
        window.alert("width: " + (sensordata / 25 * 100) + "%");
        return ("width: " + (sensordata / 25 * 100) + "%");
    }
    else if (sensordata == 'S')
        if (sensordata <= 0) {
            window.alert("width: 0%");
            return ("width: 0%");
        }
        else if (sensordata >= 90) {
            window.alert("width: 100%");
            return ("width: 100%");
        }
        else {
            window.alert("width: " + (sensordata / 90 * 100) + "%");
            return ("width: " + (sensordata / 90 * 100) + "%");
        }
    else
        window.alert("ERROR");
    return 2;
    setTimeout(progressbar, 5000);

};