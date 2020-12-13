import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';


export default class YelpSearchAndResultsComponent extends Component {
	@service locationsService;
	@service router;

	@tracked isLoading;

	get results() {
		return this.locationsService.lastSearch;
	}
	get businesses() {
		return this.results ? this.results.businesses : null;
	}
	get currentPage() {
		if (this.results) {
			return this.results ? this.results.offset / this.results.pageLength : 0;
		}
		return 0;
	}
	get offset() {
		return this.results ? this.results.offset : 0;
	}
	get more() {
		if (this.results)
			return this.results.total > this.results.offset + this.results.pageLength;
		return false;
	}
	

	@action
	searchMethod(params) {
		this.isLoading = true;
		params.radius = parseFloat(params.radius);
		return this.locationsService.getLocationsBySearch(params)
			.catch(data => alert(data))
			.finally(() => {
				window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });
				this.isLoading = false;
			});
	}

	@action
	populateResults() {
		return true;
	}

	@action
	joinCategories(loc) {
		return loc.categories.join(', ');
	}

	@action
	getNextPage() {
		const params = this.results.searchParams;
		params.offset = params.offset + params.pageLength;
		return this.searchMethod(params);
	}

	@action
	getPrevPage() {
		const params = this.results.searchParams;
		params.offset = params.offset - params.pageLength;
		return this.searchMethod(params);
	}
}