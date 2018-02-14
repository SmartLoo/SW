// Write your JavaScript code.
function progressbar(sensorID, sensordata) {
    var sensorType = sensorID[0];
    var sensordata;
    if (sensorType == 'W')
        if (sensordata == 0)
            return 0;
        else
            return 1;
    if (sensorType == 'R')
        return (sensordata / 15 * 100);
    if (sensordata == "S")
        if (sensordata <= 0)
            return 0;
        else if (sensordata >= 90)
            return 100;
        else
            return (sensordata / 90 * 100);
};