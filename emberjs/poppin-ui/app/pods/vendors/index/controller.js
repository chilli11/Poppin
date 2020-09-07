import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class AdminLocationsController extends Controller {
	@service vendorsService;
	@service router;

	@tracked results;
	@tracked isLoading;

	@action
	searchMethod(params) {
		this.isLoading = true;
		this.vendorsService.getVendorsBySearch(params)
			.then(data => this.results = data)
			.catch(data => alert(data))
			.finally(() => this.isLoading = false);
	}

	@action
	populateResults() {
		return true;
	}

	@action
	clickAction(vendor) {
		return this.router.transitionTo('admin.vendors.vendor', vendor);
	}
}
