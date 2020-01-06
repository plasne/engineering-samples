const express = require('express');
const cors = require('cors');

const app = express();
const port = 5000;

app.use(
    cors({
        origin: 'http://localhost:5000',
        credentials: true
    })
);

app.get('/hello', (req, res) => res.send('Hello World!'));

app.listen(port, () => console.log(`Example app listening on port ${port}!`));
