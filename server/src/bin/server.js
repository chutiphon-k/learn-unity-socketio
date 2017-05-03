import http from 'http'
import config from 'config'

import app from 'app'
import * as sockets from 'bin/sockets'

const server = http.Server(app)
const port = process.env.PORT || config.Api.port

server.listen(port, () => {
	console.log('[INFO] Listening on *:' + port)
})

sockets.init(server)
