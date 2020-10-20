import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminCategoriesRoute extends Route {
	@service categoriesService;

	model() {
		return this.categoriesService.getCategories();
	}
}
