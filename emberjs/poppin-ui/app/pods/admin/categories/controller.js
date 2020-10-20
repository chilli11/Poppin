import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { action, set } from '@ember/object';
import { inject as service } from '@ember/service';

export default class AdminCategoriesController extends Controller {
	@service categoriesService;
	@tracked activeCategory;
	get formCategory() {
		const cat = this.activecategory;
		return {
			name: cat.name,
			slug: cat.slug,
			parent: cat.parent,
			hereId: cat.hereId,
			relatedCSV: (cat.related || []).join(','),
			childrenCSV: (cat.children || []).join(',')
		}
	}

	@action
	addOrUpdateCategory(category) {
		const method = this.activeCategory ? 'updateCategory' : 'addCategory';
		return this.categoriesService[method](category).then(() => {
			set(this.activeCategory, null);
			return this.transitionToRoute('admin.categories');
		}).catch(e => console.error(e));
	}

	@action
	clickAction(category) {
		set(this, 'activeCategory', category);
	}
}
