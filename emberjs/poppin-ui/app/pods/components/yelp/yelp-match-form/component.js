import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action, computed } from '@ember/object';
import { inject as service } from '@ember/service';
export default class YelpMatchFormComponent extends Component {
	namespace = 'YelpMatchForm';
	@service yelpService;

	@tracked location;
	@tracked term;

	categories = [];
	@computed('categories')
	get categoryList() {
		return this.categories;
	}
	@tracked yelpCategories;
	@tracked yelpBusinesses;

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
	searchBusinesses() {
		const { term, location } = this;
		this.yelpService.getYelpBusinessSearch({ term, location })
			.then(({ businesses }) => this.yelpBusinesses = businesses.map(b => this.checkMatch(b)));
	}

	@action
	fillData(match) {		
		this.yelpBusinesses = null;
		return this.args.fillData(this.checkMatch(match));
	}
}