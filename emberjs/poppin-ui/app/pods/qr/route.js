import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class QrRoute extends Route {
	queryParams = {
		loc_id: { refreshModel: false }
	}

	model(params) {
		if (params.loc_id)
			return this.transitionTo('locations.location', params.loc_id, {
				queryParams: { qr: true }
			});
	}
}
