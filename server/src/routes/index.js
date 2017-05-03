import { Router } from 'express'

import web from 'routes/web'

const router = Router()

router.use('/', web)

export default router
