import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action, computed } from '@ember/object';
import { inject as service } from '@ember/service';

export default class YelpMatchFormComponent extends Component {
	namespace = 'YelpMatchForm';
	@service yelpService;

	@tracked location;
	@tracked term;
	@tracked _radius;
	get radius() {
		return parseInt(parseInt(this._radius || 0, 10) * 1609.34, 10).toString();
	}
	@tracked yelpCategories;

	categories = [];
	@computed('categories')
	get categoryList() {
		return this.categories;
	}

	constructor() {
		super(...arguments);
		this.yelpService.getYelpCategories()
			.then((data) => this.yelpCategories = data);
	}

	checkMatch(business) {
		business.isMatch = business.id == this.args.yelpMatchId;
		return business;
	}

	@action
	search() {
		const { term, location, categories, radius } = this;
		return this.args.searchMethod({ term, location, radius, categories: categories.map(c => c.alias).join(',') });
	}
}