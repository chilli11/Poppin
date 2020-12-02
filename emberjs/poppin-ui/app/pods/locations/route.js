import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminLocationsRoute extends Route {
	@service categoriesService;
	beforeModel() {
		if (!this.categoriesService.categories || !this.categoriesService.categories.length) {
			return this.categoriesService.getCategories();
		}
	}
}
