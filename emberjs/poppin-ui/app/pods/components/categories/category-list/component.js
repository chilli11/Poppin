import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';

export default class CategoryListComponent extends Component {
	@tracked currentSort = {
		type: null,
		direction: null
	}

	@tracked sortedCategories;
	sortCategories(s) {
		const { type, direction } = this.currentSort;
		let filter = (flip, field) => (a,b) => flip ? a[field] < b[field] : a[field] > b[field];
		const sortMethod = filter(type == s && direction == '>', s);
		set(this, 'currentSort', {
			type: type,
			direction: type == s && direction == '>' ? '<' : '>'
		});
		this.sortedCategories = this.args.categories.sort(sortMethod);
	}

	constructor() {
		super(...arguments);
		this.sortBy('slug');
	}
	
	@action
	sortBy(s) {
		this.sortCategories(s);
	}
}