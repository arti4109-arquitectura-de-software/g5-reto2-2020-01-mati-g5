'use strict';

const express = require('express');

// Constants
const PORT = 8080;
const HOST = '0.0.0.0';

app.get('/', (req, res) => {
    res.send({ status : 'Ok'});
});

app.listen(PORT, HOST);
console.log(`Running on http://${HOST}:${PORT}`);