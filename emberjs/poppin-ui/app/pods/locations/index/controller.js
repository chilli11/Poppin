import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AdminLocationsController extends Controller {
	@service locationsService;
	@service router;

	@tracked results;
	@tracked isLoading;

	@action
	searchMethod(params) {
		this.isLoading = true;
		this.locationsService.getLocationsByYelpList(params)
			.then(data => this.results = data)
			.catch(data => alert(data))
			.finally(() => this.isLoading = false);
	}

	@action
	populateResults() {
		return true;
	}

	@action
	clickAction(business) {
		return this.router.transitionTo('locations.location', business);
	}
}
