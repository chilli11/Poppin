import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';


export default class YelpSearchAndResultsComponent extends Component {
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
}