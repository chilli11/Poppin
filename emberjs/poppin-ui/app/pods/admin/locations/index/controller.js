import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AdminLocationsController extends Controller {
	@service locationsService;
	@service router;

	@tracked results;

	@action
	searchMethod(params) {
		this.locationsService.getLocationsByYelpList(params)
			.then((data) => {
				this.results = data;
				return data;
			});
	}

	@action
	populateResults() {
		return true;
	}

	@action
	clickAction(business) {
		return this.router.transitionTo('admin.locations.location', business.id);
	}
}
