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
	get radius() {
		return parseInt(parseInt(this._radius || 0, 10) * 1609.34, 10).toString();
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

	@action
	search() {
		const { term, location, categories, radius } = this;
		return this.args.searchMethod({ term, location, radius, categories: categories.map(c => c.slug).join(',') });
	}
}