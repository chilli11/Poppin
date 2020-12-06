import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action, computed } from '@ember/object';
import { inject as service } from '@ember/service';

export default class NativeSearchFormComponent extends Component {
	namespace = 'NativeSearchForm';
	@service locationsService;
	@service categoriesService

	@tracked location;
	@tracked term;
	@tracked _radius;
	@tracked pageLength = 20;
	@tracked currentPage = 0;

	get radius() {
		return parseInt(parseInt(this._radius || 0, 10) * 1609.34, 10);
	}
	@tracked poppinCategories;

	categories = [];
	@computed('categories')
	get categoryList() {
		return this.categories;
	}

	constructor() {
		super(...arguments);
		this.categoriesService.getCategories()
			.then((data) => this.poppinCategories = data);
	}

	checkMatch(business) {
		return business;
	}

	deactivate() {
		this.currentPage = 0;
	}

	@action
	search(page) {
		const offset = (page || this.currentPage) * this.pageLength;
		const { term, location, pageLength, categories, radius } = this;
		return this.args.searchMethod({ term, location, offset, pageLength, radius, categories });
	}

	@action
	nextPage() {
		this.search(this.currentPage + 1);
	}
}