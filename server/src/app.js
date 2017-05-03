import express from 'express'
import bodyParser from 'body-parser'
import nodeNotifier from 'node-notifier'
import errorhandler from 'errorhandler'
import compression from 'compression'
import cors from 'cors'

import routes from 'routes'

const app = express()

if (process.env.NODE_ENV === 'development' || process.env.NODE_ENV === undefined) {
	app.use(errorhandler({log: errorNotification}))
}

function errorNotification (err, str, req) {
	if (err) {
		let title = 'Error in ' + req.method + ' ' + req.url

		nodeNotifier.notify({
			title: title,
			message: str
		})
	}
}

app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: true }))
app.use(compression())
app.use(cors())
app.use('/', routes)

export default app
