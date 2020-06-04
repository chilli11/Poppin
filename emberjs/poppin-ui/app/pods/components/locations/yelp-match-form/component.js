import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action, computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { fetch } from 'fetch';

import YelpBusinessModel from 'poppin-ui/models/yelp-business';
import { YelpBusinessSearchParams } from 'poppin-ui/classes/yelp-search-entities';

export default class YelpMatchFormComponent extends Component {
	@service store;

	categories = [];
	@computed('categories')
	get categoryList() {
		return this.categories;
	}
	@tracked yelpCategories;

	constructor() {
		super(...arguments);
		fetch('/yelp-categories.json')
			.then(response => response.json())
			.then((data) => {
				console.log(data);
				this.yelpCategories = data;
				return data;
			});

		if (this.args.locationDTO && (!this.args.yelpMatches || !this.args.yelpMatches.length)) {
			const { locationDTO } = this.args;
			const { address } = locationDTO;
			const searchParams = new YelpBusinessSearchParams({
				term: locationDTO.name,
				location: address.city
			});
			this.store.findRecord('yelpBusinessMatch', searchParams);
		}
	}

	@action
	searchBusinesses() {

	}
}