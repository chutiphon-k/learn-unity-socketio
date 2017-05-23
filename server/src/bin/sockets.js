import SocketIO from 'socket.io'

let io
let enemies = []
let playerSpawnPoints = []
let clients = []

let init = (server) => {
	io = new SocketIO(server)
	io.on('connection', (socket) => {
		let currentPlayer = {}
		currentPlayer.name = 'unknown'

		socket.on('player connect', () => {
			console.log(`${currentPlayer.name} recv: player connect`)
			clients.map((client) => {
				socket.emit('other player connected', {
					name: client.name,
					position: client.position,
					rotation: client.rotation,
					health: client.health
				})
				console.log(`${currentPlayer.name} emit: other player connected: ${JSON.stringify(client)}`)
			})
		})

		socket.on('play', (data) => {
			console.log(`${currentPlayer.name} recv: play:: ${JSON.stringify(data)}`)
			if(clients.length === 0){
				// numberOfEnemies = data.enemySpawnPoints.length
				enemies = []
				data.enemySpawnPoints.map((enemySpawnPoint) => {
					enemies.push({
						name: Math.random().toString(36).substring(7),
						position: enemySpawnPoint.position,
						rotation: enemySpawnPoint.rotation,
						health: 100
					})
				})
				playerSpawnPoints = []
				data.playerSpawnPoints.map((_playerSpawnPoint) => {
					playerSpawnPoints.push({
						position: _playerSpawnPoint.position,
						rotation: _playerSpawnPoint.rotation
					})
				})
				let enemiesResponse = { enemies }
				console.log(`${currentPlayer.name} emit: enemies: ${JSON.stringify(enemiesResponse)}`)
				socket.emit('enemies', enemiesResponse)
				let randomSpawnPoint = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)]
				currentPlayer = {
					name: data.name,
					position: randomSpawnPoint.position,
					rotation: randomSpawnPoint.rotation,
					health: 100
				}
				clients.push(currentPlayer)
				console.log(`${currentPlayer.name} emit: play: ${JSON.stringify(currentPlayer)}`)
				socket.emit('play', currentPlayer)
				socket.broadcast.emit('other player connected', currentPlayer)
			}
		})

		socket.on('player move', (data) => {
			console.log(`recv: move: ${JSON.stringify(data)}`)
			currentPlayer.position = data.position
			socket.broadcast.emit('player move', currentPlayer)
		})

		socket.on('player turn', (data) => {
			console.log(`recv: turn: ${JSON.stringify(data)}`)
			currentPlayer.rotation = data.rotation
			socket.broadcast.emit('player turn', currentPlayer)
		})

		socket.on('player shoot', () => {
			console.log(`${currentPlayer.name} recv: shoot`)
			let data = {
				name: currentPlayer.name
			}
			console.log(`${currentPlayer.name} bcst: shoot: ${JSON.stringify(data)}`)
			socket.emit('player shoot', data)
			socket.broadcast.emit('player shoot', data)
		})

		socket.on('health', (data) => {
			console.log(`${currentPlayer.name} recv: health: ${JSON.stringify(data)}`)
			if (data.from === currentPlayer.name) {
				let indexDamaged = 0
				if (!data.isEnemy) {
					clients = clients.map((client, index) => {
						if (client.name === data.name) {
							indexDamaged = index
							client.health -= data.healthChange
						}
						return client
					})
				} else {
					enemies = enemies.map((enemy, index) => {
						if (enemy.name === data.name) {
							indexDamaged = index
							enemy.health -= data.healthChange
						}
						return enemy
					})
				}
				let response = {
					name: (!data.isEnemy) ? clients[indexDamaged].name : enemies[indexDamaged].name,
					health: (!data.isEnemy) ? clients[indexDamaged].health : enemies[indexDamaged].health
				}
				console.log(`${currentPlayer.name} bcst: health: ${JSON.stringify(response)}`)
				socket.emit('health', response)
				socket.broadcast.emit('health', response)
			}
		})

		socket.on('disconnect', () => {
			console.log(`${currentPlayer.name} recv: disconnect ${currentPlayer.name}`)
			socket.broadcast.emit('other player disconnected', currentPlayer)
			console.log(`${currentPlayer.name} bcst: other player disconnected ${currentPlayer}`)
			let index = clients.findIndex(client => client.name === currentPlayer.name)
			clients.splice(index, 1)
		})
	})
}

export {
	init,
	io
}
