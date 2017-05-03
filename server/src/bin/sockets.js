import SocketIO from 'socket.io'

let io

let init = (server) => {
	io = new SocketIO(server)
	io.on('connect', (socket) => {
		socket.emit('timer', 'hello')
	})
}

export {
	init,
	io
}
