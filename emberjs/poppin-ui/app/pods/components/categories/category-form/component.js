/* eslint-disable ember/no-observers */
import StatefulComponent from 'poppin-ui/classes/stateful-component';
import { action } from '@ember/object';
import _ from 'lodash';

export default class CategoryFormComponent extends StatefulComponent {
	get outputCategory() {
		const cat = this.args.category;
		return {
			name: cat.name,
			slug: cat.slug,
			parent: cat.parent,
			hereId: cat.hereId,
			related: (cat.relatedCSV || '').split(',').map(r => r.trim()),
			children: (cat.childrenCSV || '').split(',').map(r => r.trim())
		}
	}

	@action
	submit() {
		return this.args.submit(this.outputCategory);
	}
}